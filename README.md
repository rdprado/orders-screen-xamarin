# orders-screen-xamarin
Projeto Xamarin para teste de uma aplicação cross-platform em WPF e UWP usando um datagrid view com dados de exemplo similares a ordens do mercado de ações. Entradas podem ser adicionadas ou atualizadas na UI.

## 1- Layout ##
Diferente do WPF ou Windows Forms o Xamarin não inclui um controle de datagrid nativo e as opções seriam usar uma solução não nativa existente ou criar uma nova. Foram encontrados alguns pacotes com datagrid views, como um da SyncFusion que diz ter uma solução de alta performance, mas sua licença é comercial. Foi encontrado também o pacote akgulebubekir/Xamarin.Forms.DataGrid, não suportado por WPF. Com isso, foi decidido criar uma solução simples utilizando o controle ListView.

![main view](https://user-images.githubusercontent.com/5822726/118692225-31bd9f00-b7e0-11eb-9681-71e4a193a4b9.PNG)

No fim, o visual ficou como esperado com a ListView. Um ajuste precisou ser feito no App.xaml, apenas para WPF, para remover uma margem indesejada na esquerda da ListView. O tamanho horizontal foi fixado devido a um identificado no UWP ao redimensionar a janela enquanto se estava adicionando ordens. Poderia ter sido corrigido apenas para o UWP. Um desafio de usar a ListView foi o de como manter a coluna do cabeçalho do mesmo tamanho das colunas dos itens da ListView. Para simplificar, a largura das colunas foi definida como valores fixos.

## 2- Mock para carregar as entradas na view de ordens ##
Foi criada uma interface, OrdersScreenFeeder, e um mock que a implementa para alimentar a view. Dessa forma, foi possível injetar a implementação do mock na view de ordens bastando que o código da view chamasse um método Start() sem conhecer detalhes de implementação. Também seria possível criar outros mocks de teste ou ter implementações reais de serviços para obter ou receber dados. Com isso, também a implementação do Feeder poderia estar em um projeto separado.

A pattern MVVM foi utilizada como recomendado em https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding com o itemSource da ListView usando um binding para uma ObservableCollection. Assim, a ListView seria atualizada automaticamente quando células fossem inseridas. O ObservableCollection possui objetos OrderViewModel neste exemplo. A lista, também um viewmodel, poderia ter referencias para Order models diretamente mas como decisão de projeto foi escolhido separar a camada de modelo da view.

O mock usa um timer e a cada 50ms adiciona uma ordem na ObservableCollection ou faz um update, ou ambos. A cada 10 segundos simula um aumento de carga de updates em 30% das ordens existentes.

## 3- Memória e performance ##
Uma das primeiras impressões foi que a aplicação UWP estava performando melhor que a WPF enquanto linhas estavam sendo adicionadas no DataGrid. Quando se arrastava as janelas ou se realizava um scroll a janela em UWP se movia suavemente enquanto a janela em WPF travava um pouco. Também, examinando-se o profiler de memória do VisualStudio, foi possível notar um aumento de memória muito grande enquanto as linhas eram adicionadas. Também foi notado que com um limite para o número de ordens adicionadas, após a criação de todas as ordens, a performance da aplicação em WPF voltava a ficar boa indicando que a criação de ordens seria o gargalo e não os updates. Mesmo com a carga maior de updates de 10 em 10 segundos, a aplicação funcionava bem.

Nas aplicações existe uma thread de UI e uma thread do timer que fica no mock, gerando e atualizando ordens. Como os updates não pareceram sobrecarregar a thread da UI, as múltiplas chamadas de BeginInvoke para executar as operações de add e update na thread da UI não pareciam também ser um problema. Abaixo a análise que foi feita a partir disso:

1- Consumo de memória

Ordens visíveis na tela sem precisar de scroll: 32  

|   | 0 orders| 1000 orders | 10000 orders |
|---| --------|-------------|--------------|
|UWP|   34 MB |      290 MB |      ~ 1.3GB |
|WPF|  109 MB |      870 MB |      >   3GB |


Memory snapshots UWP
![CaptureUWP-4500ordens](https://user-images.githubusercontent.com/5822726/118740162-8af9f280-b821-11eb-8ff7-cb0533c76f38.png)


Aproximadamente 8000 ordens criadas:
![CaptureUWP-8000bordens](https://user-images.githubusercontent.com/5822726/118710222-62a7cf00-b7f4-11eb-8a6d-40cf4be90a01.PNG)

O resultado mostra que mesmo tendo sido ativada a opcão de caching strategy para reciclar células, aparentemente todas as células foram criadas, uma para cada entrada da coleção. Dessa forma, com 10000 ordens e 32 ordens visíveis, a solução com a ListView já começaria a ficar impraticável. O mesmo ocorre em WPF.

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

Alterando o código para setar o itemSource antes e adicionar as células depois, não só ficou extremamente lento durante a adição como o gasto de memória voltou a ser em torno de 1.3GB.
```
listView.ItemsSource = orders;
while (i++ < 10000)
{
    orders.Add(new OrderViewModel());
}
```
Com isso, o alvo do problema parece ter sido encontrado e está em adicionar entradas após o itemSource já ter sido definido e ao não reuso de células. A criação de células novas com todas as estruturas de evento e bindings por célula para manter a lista responsiva gastam muita memória. Foram tentadas outras versões do Xamarin.Forms, com o mesmo resultado. Também foi tentado criar células mais simples e constatado que menos campos nas células reduz o gasto de memória, mas não acaba com o problema da política de reciclagem de células não ser utilizada a todo momento.

2- CPU performance

Como mostrado anteriormente as células não foram reusadas na ListView e as alocações delas e das estruturas necessárias para manter a lista reponsiva também se tornam um gargalo para a CPU. Enquanto as ordens são criadas a performance é prejudicada. Enquanto existem updates apenas, mesmo em uma maior frequência, a responsividade continua boa.

Examplo de comportamento do processador em UWP enquanto 10000 ordens estão sendo criadas:
![image](https://user-images.githubusercontent.com/5822726/118736638-6f8ae980-b819-11eb-8197-7bae1c08c01b.png)

Exemplo quando as 10000 ordens já foram criadas:  
![image](https://user-images.githubusercontent.com/5822726/118737773-f04ae500-b81b-11eb-946a-7226a83473d8.png)

Os picos no gráfico de processamento mostram os momentos de aumento carga de updates. Mais claros quando não há mais criação de ordens. Ocorrem a cada 10 segundos e duram até completar as chamadas de updates de 30% das ordens existentes.

Comportamento semelhante no Xamarin  
![image](https://user-images.githubusercontent.com/5822726/118739498-02c71d80-b820-11eb-99f9-f9c579254e9b.png)

![image](https://user-images.githubusercontent.com/5822726/118739714-8123bf80-b820-11eb-977e-d766a90bf08c.png)

Com picos de menor duração indicando uma performance possivelmente melhor nesse caso de updates.

## Conclusão ##
Como o Xamarin não possui um controle nativo de DataGrid foi criado um utilizando-se o ListView. A política de uso de células funcionou apenas quando os elementos da coleção foram adicionados antes de a coleção ser setada como ItemSource da ListView e como é preciso adicionar elementos posteriormente, o gasto de memória foi grande. O processamento também fica comprometido enquanto se adiciona elementos. Alguns experimentos foram feitos para tentar que o reuso de células fosse feito, mas sem sucesso. A solução seria criar um componente sem usar o ListView que tivesse uma política de reuso de células, adotar uma solução comercial ou investigar mais a fundo o próprio Xamarin e o componente de ListView.

## Notas ## 
- Máquina usada para testes:
Intel(R) Core(TM) i7-9750H @2.60Hz | RAM 16GB

- Versões utilizadas  
Visual Studio 2019  | WPF Core 3.1 e WPF Framework 4.72 | Xamarin.Forms mais recente

- Compilação  
Foi constatado que com essas verões utilizadas a compilação dos projetos em Release é muito mais lenta do que em Debug

- Erros
Apenas no UWP aparece esse erro a ser investigado, apesar de funcionar ok.
![image](https://user-images.githubusercontent.com/5822726/118736305-a14f8080-b818-11eb-8507-571c4712dd82.png)

Referências  
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding  
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/performance  
https://stackoverflow.com/questions/62131069/how-can-i-clear-strange-spacing-of-listviewitem-in-xamarin-wpf  
https://help.syncfusion.com/xamarin/datagrid/getting-started  
