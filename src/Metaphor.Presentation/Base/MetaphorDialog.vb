Imports Metaphor.Processing
Imports TGGD.Presentation

Public MustInherit Class MetaphorDialog
    Inherits StackedModelDialog(Of IDisplayContext, IWorldModel)

    Protected Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub
End Class
