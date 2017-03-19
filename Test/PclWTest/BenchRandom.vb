Imports System.Collections.ObjectModel
Imports System.Math
Imports PclWCommon
Imports PclWTestCommon

Class BenchRandom
    Private _Generator As TestableObj
    Public Property Generator As TestableObj
        Get
            Return _Generator
        End Get
        Set(ByVal value As TestableObj)
            'NOTE: Gtl required to create random polygons, it gives "cleaner" polygons.
            If value.Id <> "Gtl" Then
                Throw New Exception("Gtl required.")
            End If
            _Generator = value
        End Set
    End Property

    Public Property BuildAdding As Boolean
    Public Property MaxRandomPolygonSideLenght As Double
    Public Property SideLength As Double
    Public Property RandomPolygons As Integer
    Public Property UseRectangles As Boolean


    Private _RegionA As New Region
    Private _RegionB As New Region


    Private Function GetRandomPolygon(
            ByVal maxWidth As Double, ByVal maxHeight As Double,
            ByVal deltaX As Double, ByVal deltaY As Double
            ) As Polygon
        Dim shape As Polygon
        If _UseRectangles Then
            shape = GetRandomRectangle(
                _MaxRandomPolygonSideLenght, _MaxRandomPolygonSideLenght,
                deltaX, deltaY)
        Else
            shape = GetRandomTriangle(
                _MaxRandomPolygonSideLenght, _MaxRandomPolygonSideLenght,
                deltaX, deltaY)
        End If
        Return shape
    End Function

    Private Function GetRandomRectangle(
            ByVal maxWidth As Double, ByVal maxHeight As Double,
            ByVal deltaX As Double, ByVal deltaY As Double
            ) As Polygon
        Const Delta As Double = 10

        Dim halfWidth As Double
        Do
            halfWidth = GetRandomDouble() * maxWidth / 2
        Loop While halfWidth < Delta
        Dim halfHeight As Double
        Do
            halfHeight = GetRandomDouble() * maxHeight / 2
        Loop While halfHeight < Delta

        Dim coordinates(7) As Double
        coordinates(0) = -halfWidth
        coordinates(1) = -halfHeight
        coordinates(2) = -halfWidth
        coordinates(3) = +halfHeight
        coordinates(4) = +halfWidth
        coordinates(5) = +halfHeight
        coordinates(6) = +halfWidth
        coordinates(7) = -halfHeight

        For c As Integer = 0 To coordinates.Length - 1 Step 2
            coordinates(c) = deltaX + coordinates(c)
            coordinates(c + 1) = deltaY + coordinates(c + 1)
        Next

        Dim polygon As New Polygon(coordinates)
        Return polygon
    End Function

    Private Function GetRandomTriangle(
            ByVal maxWidth As Double, ByVal maxHeight As Double,
            ByVal deltaX As Double, ByVal deltaY As Double
            ) As Polygon
        'NOTE: Decreasing this value raises chances of obtaining invalid polygons.
        Const Delta As Double = 1.5

        Dim area As Double = 0
        Dim polygon As Polygon = Nothing
        Do
            Dim coordinates(5) As Double
            coordinates(0) = GetRandomDouble() * maxWidth + deltaX
            coordinates(1) = GetRandomDouble() * maxHeight + deltaY
            coordinates(2) = GetRandomDouble() * maxWidth + deltaX
            coordinates(3) = GetRandomDouble() * maxHeight + deltaY
            coordinates(4) = GetRandomDouble() * maxWidth + deltaX
            coordinates(5) = GetRandomDouble() * maxHeight + deltaY
            polygon = New Polygon(coordinates)
            area = CalculateArea(polygon)
        Loop While Abs(area) <= Delta

        If area < 0 Then
            polygon.Reverse()
        End If

        Return polygon
    End Function

    Private Sub SetRegion(ByRef region As Region)
        If region Is Nothing Then
            region = New Region
        End If

        ReverseRegion(region)

        Dim halfSideLength As Double = _SideLength / 2

        If region.PolygonSets.Count = 0 Then
            Dim ps As New PolygonSet
            region.PolygonSets.Add(ps)

            If Not _BuildAdding Then
                Dim coordinates() As Double = {
                    -halfSideLength, -halfSideLength,
                    -halfSideLength, halfSideLength,
                    halfSideLength, halfSideLength,
                    halfSideLength, -halfSideLength
                    }
                Dim polygon As New Polygon(coordinates)
                polygon.Reverse()
                ps.Polygons.Add(polygon)
            End If
        End If

        For i As Integer = 1 To _RandomPolygons
            Dim deltaX As Double = GetRandomDouble() * halfSideLength
            Dim deltaY As Double = GetRandomDouble() * halfSideLength
            Dim p As Polygon = GetRandomPolygon(
                    _MaxRandomPolygonSideLenght, _MaxRandomPolygonSideLenght,
                    deltaX, deltaY)
            p.Reverse()

            Dim ps As New PolygonSet
            ps.Polygons.Add(p)

            Dim rT As New Region
            rT.PolygonSets.Add(ps)

            If _BuildAdding Then
                region = _Generator.GetUnion(region, rT)
            Else
                region = _Generator.GetDifference(region, rT)
            End If
        Next

        Select Case _Generator.Id
            Case "Ggl"
                '
            Case "Cgal", "Gtl"
                'Outer polygons must be clockwise.
                ReverseRegion(region)
        End Select
    End Sub

    Public Sub SetRegions()
        SetRegion(_RegionA)
        SetRegion(_RegionB)
    End Sub

    Public Sub TestSpeedRandom(ByVal testObj As TestableObj,
            ByVal loops As Integer)
        Dim semiSideLength As Double = _SideLength / 2

        Dim oA As Object = testObj.GetAdaptedInputFromRegion(_RegionA)
        Dim oB As Object = testObj.GetAdaptedInputFromRegion(_RegionB)

        Dim oC As Object = Nothing

        Dim watch As New Stopwatch
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            'Threading.Thread.Sleep(1000)
            watch.Start()
            For i As Integer = 1 To loops
                oC = testObj.GetIntersection(oA, oB)
            Next
            watch.Stop()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            If TypeOf oA Is Region Then
                oC = New Region
            Else
                oC = New PolygonSet
            End If
        End Try

        Dim psA As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oA)
        Dim psB As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oB)
        Dim psC As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oC)

        'Dim svgWriter As New SvgWriter
        'svgWriter.AdaptViewBox = True
        'Dim viewPort As SvgWriter.ViewportDefinition
        'viewPort.HeightCm = 20
        'viewPort.WidthCm = 20
        'svgWriter.Viewport = viewPort
        'svgWriter.StrokeWidth = _SideLength / 2500
        'svgWriter.Write(psA, testObj.Id & ".RNDA")
        'svgWriter.Write(psB, testObj.Id & ".RNDB")
        'svgWriter.Write(psC, testObj.Id & ".RNDC")

        Dim sourcePolygonsCount As Integer = GetPolygonsCount(psA) +
            GetPolygonsCount(psB)
        Dim sourceVerticesCount As Integer = GetVerticesCount(psA) +
            GetVerticesCount(psB)
        Dim resultPolygonsCount As Integer = GetPolygonsCount(psC)
        Dim resultVerticesCount As Integer = GetVerticesCount(psC)
        Dim message As String =
            String.Format("TR {0,-10} ", testObj.Id) &
            String.Format("l {0:0000} ", loops) &
            String.Format("ms {0:00000000} ", watch.ElapsedMilliseconds) &
            String.Format("PS {0:00000} ", sourcePolygonsCount) &
            String.Format("VS {0:00000000} ", sourceVerticesCount) &
            String.Format("PR {0:000000} ", resultPolygonsCount) &
            String.Format("VR {0:00000000}", resultVerticesCount)
        Console.WriteLine(message)
    End Sub
End Class
