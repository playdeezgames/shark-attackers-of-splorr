Imports Metaphor.Processing
Imports TGGD.Presentation

Public Class Title
    Inherits MetaphorDialog
    Implements IDialog

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides Function Run() As IDialogPrompt
        Context.Render("Handies for Cash!", New Dictionary(Of String, String) From {{HintNames.ELEMENT_TYPE, ElementTypes.TITLE}})
        Context.Render("A Production of ", newLine:=False)
        Context.Render("TheGrumpyGameDev", New Dictionary(Of String, String) From {{HintNames.ELEMENT_TYPE, ElementTypes.LINK}, {HintNames.URL, "https://thegrumpygamedev.itch.io/"}})
        Context.Render("Sponsored by: ", newLine:=False)
        Context.Render("UMLAUT.FYI!", New Dictionary(Of String, String) From {{HintNames.ELEMENT_TYPE, ElementTypes.LINK}, {HintNames.URL, "https://umlaut.fyi/"}})
        Context.Render("New Sponsor! ", newLine:=False)
        Context.Render("Pen 15!", New Dictionary(Of String, String) From {{HintNames.ELEMENT_TYPE, ElementTypes.LINK}, {HintNames.URL, "https://pen15.site/"}})
        Return DialogPrompt.CreateChoicePrompt(
            "",
            DialogChoice.Create(True, "OK", MainMenu.Launch(Context, Model, Previous)))
    End Function

    Public Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New Title(context, model, previous)
    End Function
End Class
