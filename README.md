

[![Nuget Version](https://img.shields.io/nuget/v/AspNetStandard.Diagnostics.HealthChecks?style=plastic)](https://www.nuget.org/packages/AspNetStandard.Diagnostics.HealthChecks/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AndrePostiga_AspNetStandard.Diagnostics.HealthChecks&metric=alert_status)](https://sonarcloud.io/dashboard?id=AndrePostiga_AspNetStandard.Diagnostics.HealthChecks)
![Nuget](https://img.shields.io/nuget/dt/AspNetStandard.Diagnostics.HealthChecks?label=HealthChecks%20downloads)
![Nuget](https://img.shields.io/nuget/dt/AspNetStandard.Diagnostics.HealthChecks.MongoDb?label=HealthChecks.MongoDb%20downloads)
![Nuget](https://img.shields.io/nuget/dt/AspNetStandard.Diagnostics.HealthChecks.RabbitMq?label=HealthChecks.RabbitMq%20downloads)
![Nuget](https://img.shields.io/nuget/dt/AspNetStandard.Diagnostics.HealthChecks.Redis?label=HealthChecks.Redis%20downloads)
![Nuget](https://img.shields.io/nuget/dt/AspNetStandard.Diagnostics.HealthChecks.SqlServer?label=HealthChecks.SqlServer%20downloads)



`AspnetStandard.Diagnostics.HealthCheck`
------------------------------

### `A Biblioteca`
AspnetStandard.Diagnostics.HealthCheck é uma biblioteca escrita em C# e que utiliza o framework .Net Standard com a finalidade de ser compatível com os frameworks mais antigos do .Net. 

Atualmente a Microsoft possui uma biblioteca de HealthCheck para o .Net Core e o objetivo dessa lib é trazer uma implementação com uma experiência de desenvolvimento parecida com a lib original para o .Net Framework.

 A ideia inicial da implementação surgiu a partir de uma implementação da comunidade que utilizava builders e extension methods para fazer a configuração inicial.

A biblioteca original foi desenvolvida pelo [@kpol](https://github.com/kpol) e eu peguei essa ideia para mudar algumas coisas e adicionar algumas features. Para ir para a biblioteca original [clique aqui](https://github.com/kpol/WebApi.HealthChecks)

#### `Como utilizar`
A configuração inicial dessa lib foi inspirada no startup da biblioteca oficial para o .Net Core, ela é basicamente um [Builder](https://refactoring.guru/pt-br/design-patterns/creational-patterns) que pode ser adicionada ao startup da aplicação como no exemplo da POC abaixo:

<img src="https://i.imgur.com/yOD2cIb.png">

`Endpoint`
Um endpoint opcional pode ser configurado com o método `AddHealthChecks`

`Autenticação`
O seu check também pode possuir uma camada de autenticação que utiliza uma `ApiKey`, basta passar a chave no método `UseAuthorization`

`Checks`
Para incluir um check na lista de verificação, basta passar um objeto que implemente a interface `IHealthCheck ` e adicionar uma referência de chave valor para o método `AddCheck(NomeDoHealthChek, InstânciaDoHealthCheck)`

`Logging`
Os seus checks podem ser logados a cada requisição, tenha ela falhado ou não. A biblioteca implementa o ILogger e espera que o cliente passe uma instância concreta que implemente a interface para fazer os logs

#### `Adicionando HealthChecks Personalizados`
A biblioteca ainda está em fase de desenvolvimento e provavelmente ela não possui todas as implementações de HealthCheck para todos os serviços existentes. Para garantir a generalização e extensão da biblioteca para outros casos de uso, é possível implementar uma interface do projeto, desenvolver seu próprio check e utilizar na configuração inicial da aplicação.

Para adicionar um check, você deve implementar a interface `IHealthCheck ` que retorna em seu contrato um `HealthCheckResult`. A implementação do check fica a critério do usuário, basta respeitar o contrato definido pela interface.

<img src="https://i.imgur.com/M94qCGC.png">

Se precisar de um exemplo basta ver as implementações nesse repositório. As implementações são bem parecidas com as implementações da biblioteca oficial da Microsoft, se tiver em dúvida em relação a como implementar, recomendo dar uma olhada nas implementações oficiais e adequar a interface proposta na imagem acima.

[MongoDb](https://github.com/AndrePostiga/AspNetStandard.Diagnostics.HealthChecks/tree/master/src/AspNetStandard.Diagnostics.HealthChecks.MongoDb)
[RabbitMq](https://github.com/AndrePostiga/AspNetStandard.Diagnostics.HealthChecks/tree/master/src/AspNetStandard.Diagnostics.HealthChecks.RabbitMq)

Após realizar a implementação basta adicionar na configuração inicial do check com o `AddCheck(NomeDoHealthChek, InstânciaDoHealthCheck)`

#### `Tudo feito, agora como essa biblioteca funciona na prática?`
Por baixo dos panos o HealthCheck utiliza os princípios do [ChainOfResponsability](https://refactoring.guru/pt-br/design-patterns/chain-of-responsibility) aliado ao [DelegatingHandler](https://docs.microsoft.com/pt-br/dotnet/api/system.net.http.delegatinghandler?view=net-5.0).

Basicamente quando uma requisição chega ela terá que passar por três handlers que irão tratar a requisição.

DependencyHandler → AuthenticationHandler → HealthCheckHandler

O `DependencyHandler` checa se as dependências estão resolvidas, e, caso não esteja, ele resolve e passa para o próximo handler.

O `AuthenticationHandler` verifica se o usuário configurou uma ApiKey e valida se a Key passada na requisição é igual a configurada, caso a validação esteja correta ele passa para o próximo handler, caso contrário retorna uma resposta de erro. Se na configuração inicial da aplicação o desenvolvedor não tenha configurado para usar a autenticação ele apenas passa a requisição para o próximo handler.

Obs: A `ApiKey` deve ser passada via URL conforme configurado no StartUp 
`GET /qualquer_endpoint?ApiKey=qualquer_string_de_autenticação`

Contrato do erro
```json
{
  "status_code": 403,
  "message": "ApiKey is invalid or not provided."
}
```
O `HealthCheckHandler` é o verdadeiro protagonista, ele que realiza o HealthCheck em todos os checks configurados e retorna uma resposta para o cliente. 

Ele também aceita um QueryParam que pode ser passado na forma:
`GET /qualquer_endpoint?ApiKey=qualquer_string_de_autenticação&check=mongoDb`

Os contratos que o HC retorna são semelhantes aos exemplos abaixo

##### `Resposta completa sem query parameter`
```json
{
  "entries": {
    "mongoDb": {
      "response_time": 4,
      "last_execution": "2020-12-30T14:14:09.5767174Z",
      "status": "Healthy",
      "description": "MongoDb is healthy"
    },
    "rabbitMq": {
      "response_time": 2,
      "last_execution": "2020-12-30T14:14:09.5807153Z",
      "status": "Healthy",
      "description": "RabbitMQ is healthy"
    }
  },
  "over_all_status": "Healthy",
  "total_response_time": 6
}
```

##### `Resposta completa com query parameter = mongoDbHealthCheck`
```json
{
  "response_time": 14,
  "last_execution": "2020-12-30T14:14:52.5304477Z",
  "status": "Healthy",
  "description": "MongoDb is healthy"
}
```

A aplicação está sendo desenvolvida e os próximos passos são:
 - [ ] Criar extensão para o SQL Server
 - [ ] Criar extensão para o Redis
 - [ ] Criar pipeline de CI/CD
 - [ ] Fazer integração com o SonarQube
 - [ ] Fazer deploy no Nuget.org

