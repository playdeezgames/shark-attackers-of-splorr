Imports Metaphor.Processing
Imports TGGD.Presentation

Friend MustInherit Class MetaphorPickerMenu
    Inherits BasePickerMenu(Of IDisplayContext, IWorldModel)

    Protected Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub
    Protected Overrides Sub Render()
        For Each message In Model.Messages
            Context.Render(message.Text, message.HintNames.ToDictionary(Function(x) x, Function(x) message.GetHint(x)))
        Next
    End Sub
End Class
