Imports TGGD.Presentation

Public Interface IDisplay
    Inherits IDisplayContext
    Function Start() As Task
    ReadOnly Property Running As Boolean
    ReadOnly Property Elements As IEnumerable(Of IDisplayElement)
    ReadOnly Property Prompt As IDialogPrompt
End Interface
