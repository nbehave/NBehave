[<NBehave.ActionSteps>]
module TestLib.ConfigFileActionSteps
open NBehave
open System.Configuration
open Should
    
    [<Given>]
    let ``an assembly with a matching configuration file`` (action:string) (person:string) = 
        ()

    [<When>]
    let ``the value of setting $key is read`` (key:string) =
        let value = ConfigurationManager.AppSettings.[key]
        ScenarioContext.Current.Add("configValue", value)

    [<Then>]
    let ``the value should be $actionPerformed`` (actionPerformed:string) =
        let value = ScenarioContext.Current.["configValue"] |> string
        value.ShouldStartWith(actionPerformed)
