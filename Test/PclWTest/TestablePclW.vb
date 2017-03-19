Imports PclWCommon
Imports PclWTestCommon


Public Class TestablePclW : Inherits PclWTestCommon.TestableObj
    Dim _wrapper As Object

    Public Sub New(ByVal id As String,
            ByVal wrapper As Object,
            ByVal closedPolygonsRequired As Boolean,
            ByVal inputAdapterFromPolygonSet As Object,
            ByVal inputAdapterFromRegion As Object,
            ByVal outputAdapter As Object)
        Me.Id = id

        Me.ClosedPolygonsRequired = closedPolygonsRequired
        Me.InputAdapterFromPolygonSet = inputAdapterFromPolygonSet
        Me.InputAdapterFromRegion = inputAdapterFromRegion
        Me.OutputAdapter = outputAdapter

        _wrapper = wrapper
    End Sub


    Public Overloads Overrides Function GetAdaptedInputFromPolygonSet(ByVal input As PclWCommon.PolygonSet) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If
        Return Me.InputAdapterFromPolygonSet.GetAdaptedInput(input)
    End Function

    Public Overrides Function GetAdaptedInputFromRegion(ByVal input As PclWCommon.Region) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If
        Return Me.InputAdapterFromRegion.GetAdaptedInput(input)
    End Function

    Public Overrides Function GetAdaptedOutputToPolygonSet(ByVal output As Object) As PolygonSet
        Return Me.OutputAdapter.GetAdaptedOutput(output)
    End Function

    Public Overrides Function GetDifference(ByVal subject As Object, ByVal clip As Object) As Object
        Return _wrapper.GetDifference(subject, clip)
    End Function

    Public Overrides Function GetIntersection(ByVal subject As Object, ByVal clip As Object) As Object
        Return _wrapper.GetIntersection(subject, clip)
    End Function

    Public Overrides Function GetUnion(ByVal subject As Object, ByVal clip As Object) As Object
        Return _wrapper.GetUnion(subject, clip)
    End Function

    Public Overrides Function GetXor(ByVal subject As Object, ByVal clip As Object) As Object
        Return _wrapper.GetXor(subject, clip)
    End Function
End Class
