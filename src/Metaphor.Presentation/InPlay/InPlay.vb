Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class InPlay
    Inherits MetaphorDialog

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New InPlay(context, model, previous)
    End Function

    Private Delegate Function LaunchDelegate(
                                     context As IDisplayContext,
                                     model As IWorldModel,
                                     previous As DialogSource) As DialogSource

    Private modeLaunchers As New Dictionary(Of String, LaunchDelegate) From
        {
        }

    Public Overrides Function Run() As IDialogPrompt
        If Model.Ad.InProgress Then
            Return AdPrompt.Launch(Context, Model, Previous).Invoke().Run()
        End If
        Dim launchDelgate As LaunchDelegate = Nothing
        If modeLaunchers.TryGetValue(Model.Avatar.DialogMode, launchDelgate) Then
            Return launchDelgate.Invoke(Context, Model, Previous).Invoke.Run()
        End If
        Return NavigationMenu.Launch(Context, Model, Previous).Invoke().Run()
    End Function

    Friend Shared Function ChooseGameMenu(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Gämë Mënü", GameMenu.Launch(context, model, previous))
    End Function

    Friend Shared Function ChooseWatchAd(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Watch Ad...", WatchAdActivity.Launch(context, model, previous))
    End Function
End Class
