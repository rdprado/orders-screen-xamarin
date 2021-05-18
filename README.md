# orders-screen-xamarin
Projeto Xamarin para testar a criação de uma aplicação cross-platform para WPF e UWP usando um datagrid view contendo dados de ordens do mercado de ações. Entradas podem ser adicionadas ou atualizadas na view.

## 1- Layout ##
A primeira fase foi construir uma tela com ordens em uma view com estilo de datagrid. Diferente do WPF ou Windows Forms o Xamarin não inclui um controle de datagrid nativo e, então, as opções seriam usar uma solução não nativa existente contra criar uma nova. Foram encontrados alguns pacotes com datagrid views, como da SyncFusion que diz ter uma solução de alta performance, mas sua licença é comercial. Foi encontrado também um pacote akgulebubekir/Xamarin.Forms.DataGrid não suportado por WPF. Então, foi decidido criar uma solução simples utilizando o controle ListView.

![main view](https://user-images.githubusercontent.com/5822726/118692225-31bd9f00-b7e0-11eb-9681-71e4a193a4b9.PNG)

No fim, o visual ficou como esperado com a ListView. Um ajuste precisou ser feito para no App.xaml apenas para WPF, para remover uma margem indesejada na esquerda da ListView. O tamanho horizontal foi fixado pois um bug foi identificado no UWP ao redimensionar a janela enquanto se estava adicionando ordens. Poderia ter sido corrigido apenas para o UWP. Um desafio de usar a ListView foi como manter a coluna do cabeçalho do mesmo tamanho das colunas dos itens da ListView. Para simplificar a largura das colunas foi definida como valores fixos.

## 2- Mock para carregar as entradas na view de ordens ##
Para essa fase foi criada uma interface chamada OrdersScreenFeeder e um mock que a implementa, para alimentar a view. Dessa forma foi possível injetar a implementação do mock através do App.xaml na view de ordens, bastando que o código da view chamasse um método Start() sem conehcer os detalhes de implementação. Assim, também seria possível criar outros mocks de teste ou ter implementações reais de serviços para obter ou receber dados. Com isso também a implementação do Feeder poderia estar em um projeto separado.

A pattern MVVM foi utilizada como recomendado em https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding com o itemSource da ListView usando binding para uma ObservableCollection para que a ListView fosse atualizada automaticamente quando células fossem inseridas. O ObservableCollection OrdersViewModel possui OrderViewModels neste exemplo. A lsita poderia ter referencias para Order models diretamentem mas como decisão de projeto foi escolhido separar a camada de modelo da de view.

O mock usa um timer e a cada 50ms adiciona uma ordem na ObservableCollection ou faz um update ou ambos. E a cada 10 segundos simula um aumento de carga de updates em cima de 30% das ordens existentes.

## 3- Memory consumption and performance ##
As primeiras impressões foram que a aplicação UWP estava performando melhor que a em WPF enquanto linhas estavam sendo adicionadas no DataGrid. Quando se arrastava as janelas ou se realizava um scroll, a janela em UWP se movia suavemente enquanto em WPF travando um pouco. Também, examinando-se o profiler de memória do VisualStudio, foi possível notar um aumento de memória muito grande enquanto as linhas eram adicionadas. Mas, também foi notado que com um limite para o número de ordens adicionadas, após a criação de todas as ordens, a performance da aplciação em WPF voltava a ficar boa indicando que a criação de ordens seria o gargalo e não os updates. Mesmo com a carga maior de updates de 10 em 10 segundos, a aplicação funcionava bem.

Nas aplicações existe uma thread de UI e uma thread do timer que fica no mock, gerando e atualizando ordens. Como os updates não pareceram sobrecarregar a thread da UI, as múltiplas chamadas BeginInvoke para executar as operações de add e update na UI não pareciam também sr um problema. Abaixo a análise que foi feita a partir disso:

1- Consumo de memória

Ordens visíveis na tela sem precisar de scroll: 32  

|   | 0 orders| 1000 orders | 10000 orders |
|---| --------|-------------|--------------|
|UWP| 34 MB   | 290 MB      |     1.3GB    |
|WPF| 109 MB  | 870 MB      |     > 3GB    |


Memory snapshots UWP

Aproximadamente 4200 ordens criadas
![CaptureUWP-4500ordens](https://user-images.githubusercontent.com/5822726/118710154-4c9a0e80-b7f4-11eb-97aa-15938d303246.PNG)
:

Aproximadamente 8000 ordens criadas
![CaptureUWP-8000bordens](https://user-images.githubusercontent.com/5822726/118710222-62a7cf00-b7f4-11eb-8a6d-40cf4be90a01.PNG)

O resultado mostra que mesmo tendo sido ativada a opcão de caching strategy para reciclar células, aparentemente todas as células foram criadas, uma para cada linha. Dessa forma, com 10000 ordens e 32 ordens visíveis, a solução já começaria a ficar impraticável.

O teste seguinte foi criar 10000 ordens e somente depois setar o itemsource do listView:

```
while (i++ < 10000)
{
    orders.Add(new OrderViewModel());
}

listView.ItemsSource = orders;
```

No UWP, o consumo de memória foi de 229MB, uma redução considerável e pelo snapshot de memória foi possível ver que somente 256 células foram criadas e não as 10000.
![image](https://user-images.githubusercontent.com/5822726/118725389-46ad2900-b806-11eb-97e2-497d97f2cf7d.png)

Mudando o código para setar o itemSource antes e adicionar as células depois, não só ficou extremamente lento durante a adição como o gasto de memória voltou a ser em torno de 1.3GB.
```
listView.ItemsSource = orders;
while (i++ < 10000)
{
    orders.Add(new OrderViewModel());
}
```
Com isso foi possível concluir que o problema está em adicionar entradas após o itemSource já ter sido setado, como seria desejado um datagrid de ordens pois é preciso ver a lista na UI sendo atualizada a medida em que novas ordens chegam.


2- CPU performance
