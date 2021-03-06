# orders-screen-xamarin
Projeto Xamarin para teste de aplicação cross-platform em WPF e UWP. Utiliza datagrid view com dados de exemplo similares a ordens do mercado de ações. Entradas podem ser adicionadas ou atualizadas na UI.

**Para executar o projeto UWP no Visual Studio, ir em configuration manager e marcar as opções build e deploy caso não estejam selecionadas, para release e debug. Aparentemente o VS não está retendo a configuração.**

![image](https://user-images.githubusercontent.com/5822726/118900475-69fbd500-b8e7-11eb-8f8b-e9236830c816.png)

## 1- Layout ##
Diferente do WPF ou Windows Forms o Xamarin não inclui um controle de datagrid nativo e as opções seriam usar uma solução não nativa existente ou criar uma nova. Foram encontrados alguns pacotes com datagrid views, como um da SyncFusion, que diz ter uma solução de alta performance cuja licença é comercial. Foi encontrado, também, o pacote akgulebubekir/Xamarin.Forms.DataGrid, não suportado por WPF. Foi decidido criar uma solução simples utilizando o controle ListView.

![main view](https://user-images.githubusercontent.com/5822726/118692225-31bd9f00-b7e0-11eb-9681-71e4a193a4b9.PNG)

O visual ficou como esperado. Um ajuste precisou ser feito apenas para WPF, para remover uma margem indesejada na esquerda da ListView. O tamanho horizontal foi fixado devido a um bug identificado no UWP ao redimensionar a janela enquanto se estava adicionando itens. Poderia ter sido corrigido apenas para UWP. Uma questão com a ListView foi o de manter a coluna do cabeçalho (elemento separado) do tamanho das colunas da ListView. Para simplificar, a largura das colunas foi definida como valores fixos.

## 2- Mock para carregar as entradas na view de ordens ##
Criada interface OrdersScreenFeeder e um mock que a implementa para alimentar a UI. Dessa forma, foi possível injetar a implementação do mock na view de ordens sem que ela conhecesse detalhes de implementação. Também seria possível criar outros mocks de teste ou implementações reais, como de serviços para obter ou receber dados. A implementação também poderia estar em projeto separado.

A pattern MVVM foi utilizada como recomendado em https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding com o itemSource da ListView usando um binding para uma ObservableCollection da seguinte forma: 

View -> ListView  
ViewModel -> OrdersViewModel< OrderViewModel> (ObservableCollection)  
ViewModel -> OrderViewModel  

A coleção poderia ter referencias direto para o Model mas como decisão de projeto foi escolhido separar a camada de modelo da view e, portanto, existe um Order model e um view model.

O mock usa um timer e a cada 50ms adiciona uma ordem na ObservableCollection ou faz um update, ou ambos. A cada 10 segundos simula um aumento de carga de updates em 30% das ordens existentes.

## 3- Memória e performance ##
A primeira impressão foi que a aplicação UWP estava performando melhor que a WPF enquanto linhas estavam sendo adicionadas no DataGrid. Quando se arrastava as janelas ou se realizava scroll a janela em UWP se movia suavemente enquanto a janela em WPF travava um pouco. Também, examinando o profiler de memória do VisualStudio foi notado um aumento considerável de gasto de memória enquanto as linhas eram adicionadas. Também foi verificado que com um limite para o número de ordens adicionadas, após a criação de todas as ordens, a performance da aplicação em WPF voltava a ficar boa indicando que a criação de ordens seria o gargalo e não os updates. Mesmo com a carga maior de updates de 10 em 10 segundos.

Nas aplicações existe uma thread de UI e uma thread do timer que fica no mock, gerando e atualizando ordens. Como os updates não pareceram sobrecarregar a thread da UI, as múltiplas chamadas de BeginInvoke para executar as operações de add e update na thread de UI não pareciam também ser um problema. Abaixo a análise que foi feita a partir disso:

1- Consumo de memória

|   | 0 orders| 1000 orders | 10000 orders |
|---| --------|-------------|--------------|
|UWP|   34 MB |      290 MB |      ~ 1.3GB |
|WPF|  109 MB |      870 MB |      >   3GB |


Memory snapshots UWP  
    
Aproximadamente 4000 ordens criadas:  
![CaptureUWP-4500ordens](https://user-images.githubusercontent.com/5822726/118740162-8af9f280-b821-11eb-8ff7-cb0533c76f38.png)

Aproximadamente 8000 ordens criadas:  
![CaptureUWP-8000bordens](https://user-images.githubusercontent.com/5822726/118710222-62a7cf00-b7f4-11eb-8a6d-40cf4be90a01.PNG)

Os snapshots mostram que mesmo tendo sido ativada a opcão de caching strategy para reciclar células, aparentemente todas as células foram criadas, uma para cada entrada da coleção. As outras estruturas gastando mais memória são relativas aos eventos e bindings das células criadas. Dessa forma, com 10000 ordens e 32 ordens visíveis, a solução com a ListView já  ficaria impraticável, tendo criado 10000 ViewCells. O mesmo ocorre em WPF.

Para confirmar o resultado acima e que a ListView precisa conseguir reciclar as células, o teste a seguir foi feito criando-se 10000 ordens e somente depois setando o itemsource do listView:

```
while (i++ < 10000)
{
    orders.Add(new OrderViewModel());
}
listView.ItemsSource = orders;
```

Assim, no UWP, por exemplo, o consumo de memória foi de 229MB, uma redução considerável e pelo snapshot de memória foi possível ver que somente 256 células foram criadas e não 10000.
![image](https://user-images.githubusercontent.com/5822726/118725389-46ad2900-b806-11eb-97e2-497d97f2cf7d.png)

Alterando o código para setar o itemSource antes e adicionar as células depois, não só ficou extremamente lento durante a adição como o gasto de memória voltou a ser em torno de 1.3GB.
```
listView.ItemsSource = orders;
while (i++ < 10000)
{
    orders.Add(new OrderViewModel());
}
```
Com isso, o alvo do problema parece ter sido encontrado e está em adicionar entradas após o itemSource já ter sido definido e o não reuso de células. A criação de células novas com todas as estruturas de evento e bindings por célula para manter a lista responsiva gastam muita memória. Foram tentadas outras versões do Xamarin.Forms, com o mesmo resultado. Também foi tentado criar células mais simples e constatado que menos campos nas células reduz o gasto de memória, mas não acaba com o problema da política de reciclagem de células não ser utilizada a todo momento.

2- CPU performance

Como mostrado anteriormente, as células não foram reusadas na ListView e as alocações delas e das estruturas necessárias para manter a lista reponsiva também se tornaram um gargalo para a CPU. Enquanto as ordens são criadas a performance é prejudicada. Enquanto existem updates apenas, mesmo em uma maior frequência, a responsividade continua boa, mesmo no WPF, com 10000 ordens e updates.

**UWP**  
Exemplo de comportamento do processador em UWP enquanto 10000 ordens estão sendo criadas:
![image](https://user-images.githubusercontent.com/5822726/118736638-6f8ae980-b819-11eb-8197-7bae1c08c01b.png)

Exemplo quando as 10000 ordens já foram criadas:  
![image](https://user-images.githubusercontent.com/5822726/118737773-f04ae500-b81b-11eb-946a-7226a83473d8.png)

Os picos no gráfico de processamento mostram os momentos de aumento de carga de updates. Mais claros quando já terminou a criação de ordens. Ocorrem a cada 10 segundos e duram até completar as chamadas de update de 30% das ordens existentes.

**WPF**  
Comportamento no Xamarin enquanto as ordens estão sendo criadas:  
![image](https://user-images.githubusercontent.com/5822726/118739498-02c71d80-b820-11eb-99f9-f9c579254e9b.png)  

E quando todas já foram criadas, com picos de menor duração:  
![image](https://user-images.githubusercontent.com/5822726/118739714-8123bf80-b820-11eb-977e-d766a90bf08c.png)  

**Picos de CPU isolados no WPF**  
Com o profiler de CPU do Visual Studio foi possível isolar exatamente um desses picos:
![image](https://user-images.githubusercontent.com/5822726/118870336-d0b7c900-b8bc-11eb-8454-970c191cd108.png)

Praticamente metade do consumo fica na busca da ordem a ser atualizada na coleção de ordens. A busca se dá pelo ID da ordem e tem uma complexidade O(n):  
![image](https://user-images.githubusercontent.com/5822726/118870078-8b939700-b8bc-11eb-926c-ecea8a2163dd.png)  

A outra metade fica com a thread da UI:  
![Capturecpu3](https://user-images.githubusercontent.com/5822726/118870113-90f0e180-b8bc-11eb-875e-7be2adf18d7d.PNG)  

Dessa forma, caso o problema de reuso de células fosse resolvido, o update poderia ser atacado, mas a princípio mais updates alargariam o pico e não necessariamente comprometeriam o desempenho.

## Conclusão ##
Como o Xamarin não possui um controle nativo de DataGrid foi criado um utilizando-se o ListView. A política de reuso de células funcionou apenas quando os elementos da ObservableCollection foram adicionados antes de a coleção ser setada como ItemSource da ListView. Porém, como é preciso adicionar elementos posteriormente, o gasto de memória foi grande. O processamento também ficou comprometido enquanto se adicionava elementos, provavelmente por grandes alocações. Alguns experimentos foram feitos para tentar que o reuso de células fosse realizado, mas sem sucesso.  
Não parece ser o caso que a ideia da ListView seja parar de reusar células após adições. Seria, portanto, importante continuar investigando como fazer isso acontecer, mesmo que fosse preciso alterar o código do Xamarin quando recebe o evento CollectionChanged. Com a estratégia de reuso funcionando, seria viável usar a ListView para um número considerável de ordens. Uma outra solução seria criar um componente sem usar o ListView, que usasse, por exemplo, um Grid. Mas, daí seria preciso implementar uma estratégia de reuso de células e de responsividade com o MVVM.  
A partir daí, se necessário, poderia ser atacada a performance de updates quando há carga maior. Nesse caso, poderia ser melhorada a busca com alguma espécie de indexação e para a thread da UI, uma tentativa seria acumular as chamadas de update e reduzir o número de chamadas de BeginInvoke.

## Notas ## 
- Máquina usada para testes:
Intel(R) Core(TM) i7-9750H @2.60Hz | RAM 16GB

- Versões utilizadas  
Visual Studio 2019  | WPF Core 3.1 e WPF Framework 4.72 | Xamarin.Forms 5.0.0.2012

- Compilação  
Nas primeiras vezes que se compila o projeto UWP a compilação pode demorar e ainda não foi possível identificar o motivo.

- Erros
Apenas no UWP aparece o erro abaixo, a ser investigado, apesar de aplicação rodar e fazer os binds.
![image](https://user-images.githubusercontent.com/5822726/118736305-a14f8080-b818-11eb-8507-571c4712dd82.png)

Referências  
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding  
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/performance  
https://stackoverflow.com/questions/62131069/how-can-i-clear-strange-spacing-of-listviewitem-in-xamarin-wpf  
https://help.syncfusion.com/xamarin/datagrid/getting-started  
