Imports Metaphor.Persistence
Imports TGGD.Extensions

Friend Class AdModel
    Implements IAdModel

    Private ReadOnly world As IWorld

    Private Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property InProgress As Boolean Implements IAdModel.InProgress
        Get
            Return world.AdFinish.HasValue
        End Get
    End Property

    Private Delegate Sub AddShower(world As IWorld)

    Private ReadOnly addShowers As New Dictionary(Of AddShower, Integer) From
        {
            {AddressOf ShowUmlautFyiAd, 1},
            {AddressOf ShowPen15SiteAd, 1}
        }

    Private Sub ShowUmlautFyiAd(world As IWorld)
        world.AddMessage(
            "For all yer umlauting needs! umlaut.fyi",
            New Dictionary(Of String, String) From
            {
                {"ELEMENT_TYPE", "LINK"},
                {"URL", "https://umlaut.fyi/"}
            })
    End Sub

    Private Sub ShowPen15SiteAd(world As IWorld)
        world.AddMessage(
            "Everybody loves Pen 15!",
            New Dictionary(Of String, String) From
            {
                {"ELEMENT_TYPE", "LINK"},
                {"URL", "https://pen15.site/"}
            })
    End Sub

    Public Sub Show() Implements IAdModel.Show
        world.ClearMessages()
        If world.AdFinish.Value > DateTimeOffset.Now Then
            Dim timeRemaining = world.AdFinish.Value - DateTimeOffset.Now
            world.AddMessage($"Time left in ad break: {timeRemaining.ToString("mm\:ss")}")
            world.AddMessage("(This is a turn based game. As such, this counter will not automatically change. You have to click the OK button to refresh.)")
            RNG.FromGenerator(addShowers).Invoke(world)
        Else
            world.AddMessage("Ad break is complete! You may return to yer metaphor!")
            Dim avatar = world.Avatar
            world.AdFinish = Nothing
        End If
    End Sub

    Public Sub Start() Implements IAdModel.Start
        world.AdFinish = DateTimeOffset.Now.AddMinutes(2.0)
    End Sub

    Friend Shared Function Create(entity As IWorld) As IAdModel
        Return New AdModel(entity)
    End Function
End Class
