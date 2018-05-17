[<NBehave.ActionSteps>]
module HelloSteps
open NBehave
open NBehave.Hooks
open System.Configuration
open Should

    [<BeforeScenario>]
    let Setup() =
        FeatureContext.Current.Add("hellos", [])

    [<Given>]
    let ``an action:`` action person =
        let hellos = FeatureContext.Current.Get<list<string>>("hellos")
        let x = sprintf "%s, %s" action person
        FeatureContext.Current.Add("hellos", x::hellos)
        
        
    [<When(@"do it$")>]
    let ``do it`` () =
        ()
        
    [<Then>]
    let ``actions performed are:`` actionPerformed =
        let hellos = FeatureContext.Current.Get<list<string>>("hellos")
        hellos.ShouldContain(actionPerformed);
