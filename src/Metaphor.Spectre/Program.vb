Imports System.Windows
Imports Metaphor.Platform
Imports Metaphor.Presentation
Imports Spectre.Console
Imports TGGD.Platform
Imports TGGD.Presentation

Friend Delegate Function ElementRenderer(element As IDisplayElement) As Boolean

Module Program
    Sub Main(args As String())
        Console.Title = "Shark Attackers of SPLORR!!"
        Dim display As IDisplay = MetaphorDisplay.Create(True, New Persister).Result
        While display.Running
            AnsiConsole.Clear()
            For Each element In display.Elements
                RenderElement(element)
            Next
            ReadPrompt(display.Prompt)
        End While
    End Sub

    Private Sub ReadPrompt(prompt As IDialogPrompt)
        Select Case prompt.PromptType
            Case DialogPromptType.PROMPT_CHOOSE
                ReadChoosePrompt(prompt)
            Case DialogPromptType.PROMPT_DOUBLE
                ReadDoublePrompt(prompt)
            Case DialogPromptType.PROMPT_INTEGER
                ReadIntegerPrompt(prompt)
            Case DialogPromptType.PROMPT_STRING
                ReadStringPrompt(prompt)
            Case Else
                Throw New NotImplementedException
        End Select
    End Sub

    Private Sub ReadStringPrompt(prompt As IDialogPrompt)
        prompt.Respond(text:=AnsiConsole.Ask(Of String)($"[olive]{Markup.Escape(prompt.Title)}[/]"))
    End Sub

    Private Sub ReadIntegerPrompt(prompt As IDialogPrompt)
        prompt.Respond(counter:=AnsiConsole.Ask(Of Integer)($"[olive]{Markup.Escape(prompt.Title)}[/]"))
    End Sub

    Private Sub ReadDoublePrompt(prompt As IDialogPrompt)
        prompt.Respond(dimension:=AnsiConsole.Ask(Of Double)($"[olive]{Markup.Escape(prompt.Title)}[/]"))
    End Sub

    Private Sub ReadChoosePrompt(prompt As IDialogPrompt)
        Dim selectionPrompt As New SelectionPrompt(Of Integer) With
            {
                .Title = $"[olive]{Markup.Escape(prompt.Title)}[/]",
                .Converter = Function(x) prompt.Choices(x)
            }
        selectionPrompt.AddChoices(Enumerable.Range(0, prompt.Choices.Length))
        prompt.Respond(counter:=AnsiConsole.Prompt(selectionPrompt))
    End Sub

    Private ReadOnly elementRenders As IEnumerable(Of ElementRenderer) =
        {
            AddressOf TitleElementRenderer,
            AddressOf LinkElementRenderer,
            AddressOf DefaultElementRenderer
        }
    Private Function LinkElementRenderer(element As IDisplayElement) As Boolean
        Dim elementType As String = Nothing
        If Not If(element.Hints?.TryGetValue(HintNames.ELEMENT_TYPE, elementType), False) Then
            Return False
        End If
        If elementType <> ElementTypes.LINK Then
            Return False
        End If
        AnsiConsole.Markup($"{Markup.Escape(element.Text)}([blue]{Markup.Escape(element.Hints(HintNames.URL))}[/])")
        If element.NewLine Then
            AnsiConsole.WriteLine()
        End If
        Return True
    End Function

    Private Function TitleElementRenderer(element As IDisplayElement) As Boolean
        Dim elementType As String = Nothing
        If Not If(element.Hints?.TryGetValue(HintNames.ELEMENT_TYPE, elementType), False) Then
            Return False
        End If
        If elementType <> ElementTypes.TITLE Then
            Return False
        End If
        Dim figlet As New FigletText(element.Text) With
            {
                .Color = Color.Fuchsia,
                .Justification = Justify.Center
            }
        AnsiConsole.Write(figlet)
        Return True
    End Function

    Private Function DefaultElementRenderer(element As IDisplayElement) As Boolean
        AnsiConsole.Markup(Markup.Escape(element.Text))
        If element.NewLine Then
            AnsiConsole.WriteLine()
        End If
        Return True
    End Function

    Private Sub RenderElement(element As IDisplayElement)
        For Each renderer In elementRenders
            If renderer.Invoke(element) Then
                Return
            End If
        Next
    End Sub
End Module
