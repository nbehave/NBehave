[<NBehave.ActionSteps>]
module GreetingSystemSteps
open NBehave
open NBehave.Hooks
open System.Configuration
open Should
   
   type GreetingSystem() =
        [<DefaultValue>] val mutable Name:string
        [<DefaultValue>] val mutable Greeting:string

        member this.GiveName name = 
            this.Name <- name
        
        member this.BuildGreeting () =
            this.Greeting <- sprintf "“Hello, %s!”" this.Name

    let greetingSystem () =
        ScenarioContext.Current.["greetings"] :?> GreetingSystem

    [<Given>]
    let ``my name is $name`` name =
        let greetingSystem = new GreetingSystem()
        greetingSystem.GiveName name
        ScenarioContext.Current.Add("greetings", greetingSystem)

    [<When>]
    let ``I'm greeted`` () =
        greetingSystem().BuildGreeting()

    [<Then>]
    let ``I should be greeted with “Hello, $name!”$`` name =
        (sprintf "“Hello, %s!”" name).ShouldStartWith(greetingSystem().Greeting)
