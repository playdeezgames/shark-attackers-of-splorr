Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Module StatusActivity
    Friend Function LaunchStatusActivity(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function()
                   model.Avatar.ShowStatus()
                   Return InPlay.Launch(context, model, previous).Invoke()
               End Function
    End Function
End Module
