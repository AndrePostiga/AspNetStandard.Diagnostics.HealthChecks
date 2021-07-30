
`AspnetStandard.Diagnostics.HealthChecks.Wcf`
------------------------------

### `A Biblioteca`
AspnetStandard.Diagnostics.HealthChecks.Wcf é uma extensão da biblioteca AspnetStandard.Diagnostics.HealthCheck para permitir a criação de HealthChecks para
aplicações WCF (Windows Communication Foundation).

### `Como utilizar`
A configuração da biblioteca é feita adicionando no Application_Start do Global.asax, uma chamada para o método AddWcfHealthCheck da classe WcfHeathCheckRouteExtension.
Esse método é responsável por adicionar uma nova rota de HealthCheck à tabela de rotas da aplicação, e também retorna a instância de um builder que será responsável
por adicionar as dependências da aplicação que serão análisadas no HealthCheck.

#### Adicionando dependências
Para adicionar as dependências da aplicação, é necessário utilizar o método AddCheck da classe WcfHealthCheckBuilder, passando como parâmetro o nome da dependência
e uma implementação da interface IHealthCheck que será responsável por toda a lógica de verificação do status dessa dependência.

Por padrão, será necessário adicionar ao menos uma dependência passando uma instância da classe WcfHealthCheck, que é a classe responsável pelo retorno do Status da aplicação.

#### Exemplo de utilização
Abaixo usaremos como exemplo uma aplicação que tenha como dependência o SqlServer. Logo, iremos realizar o setup do HealthCheck chamando o método AddWcfHealthCheck,
e a partir do padrão builder construir nosso HealthCheck.

Para adicionar a dependência do SqlServer, vamos utilizar biblioteca `AspnetStandard.Diagnostics.HealthChecks.SqlServer`, que já possui a implementação da interface IHealthCheck
e que precisa receber uma connectionString e um script SQL para ser executado.

O código irá ficar da seguinte maneira:

```csharp
public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WcfHeathCheckRouteExtension.AddWcfHealthCheck() // Adiciona a rota de HealthCheck
                .AddCheck("MyApi", new WcfHealthCheck()) // Faz o setup para o HealthCheck WCF
                .AddCheck("SqlServer",
                            new SqlServerHealthCheck(
                                ConfigurationManager.ConnectionStrings["MyDataBase"].ToString(),
                                "SELECT TOP 1 Id FROM MyTable")); // Faz o setup para o HealthCheck SqlServer


            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
``` 
Nesse exemplo estamos passando como parâmetro uma connectionString armazenada no config da aplicação.