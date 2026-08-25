Imports TGGD.Presentation

Public MustInherit Class Display
    Implements IDisplay

    Private _prompt As IDialogPrompt
    Private ReadOnly _elements As New List(Of IDisplayElement)

    Protected Sub New()
    End Sub

    Public ReadOnly Property Running As Boolean Implements IDisplay.Running
        Get
            Return _prompt IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property Elements As IEnumerable(Of IDisplayElement) Implements IDisplay.Elements
        Get
            Return _elements
        End Get
    End Property

    Public ReadOnly Property Prompt As IDialogPrompt Implements IDisplay.Prompt
        Get
            Return New WrappedPrompt(_prompt, AddressOf UpdateDialog)
        End Get
    End Property

    Protected Sub UpdateDialog(dialog As IDialog)
        _elements.Clear()
        _prompt = dialog?.Run()
    End Sub

    Public Sub Render(
                     Optional text As String = Nothing,
                     Optional hints As IReadOnlyDictionary(Of String, String) = Nothing,
                     Optional newLine As Boolean = True) Implements IDisplayContext.Render
        _elements.Add(DisplayElement.Create(text, hints, newLine))
    End Sub

    Public MustOverride Function Start() As Task Implements IDisplay.Start
End Class
