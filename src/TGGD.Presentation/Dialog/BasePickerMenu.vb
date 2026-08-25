Imports TGGD.Processing

Public MustInherit Class BasePickerMenu(Of TContext As IDisplayContext, TModel As IModel)
    Inherits StackedModelDialog(Of TContext, TModel)
    Public Delegate Function LaunchDelegate(
                                     context As TContext,
                                     model As TModel,
                                     previous As DialogSource) As IDialogChoice

    MustOverride ReadOnly Property PromptText As String

    Protected Sub New(context As TContext, model As TModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Protected MustOverride ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)

    Public Overrides Function Run() As IDialogPrompt
        Render()
        Return DialogPrompt.CreateChoicePrompt(
            PromptText,
            Launchers.Select(Function(x) x(Context, Model, Previous)).ToArray)
    End Function

    Protected MustOverride Sub Render()
End Class
