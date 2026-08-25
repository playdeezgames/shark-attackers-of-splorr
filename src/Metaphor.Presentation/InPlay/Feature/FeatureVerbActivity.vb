Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class FeatureVerbActivity
    Inherits MetaphorDialog

    Private ReadOnly featureModel As IFeatureModel
    Private ReadOnly verbModel As IVerbModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, verbModel As IVerbModel)
        MyBase.New(context, model, previous)
        Me.featureModel = featureModel
        Me.verbModel = verbModel
    End Sub

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, featureModel As IFeatureModel, verbModel As IVerbModel) As DialogSource
        Return Function()
                   Return New FeatureVerbActivity(c, m, p, featureModel, verbModel)
               End Function
    End Function

    Public Overrides Function Run() As IDialogPrompt
        verbModel.Perform()
        Return InPlay.Launch(Context, Model, Previous).Invoke().Run
    End Function
End Class
