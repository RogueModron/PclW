Imports PclWCommon
Imports PclWTestCommon

Class BenchSelfIntersecting
    Public Property SideLength As Double


    Private _PolygonSetA As PolygonSet
    Private _PolygonSetB As PolygonSet


    Private Function GetSelfIntersectingRandomPolygon(
            ByVal maxWidth As Double, ByVal maxHeight As Double,
            ByVal verticesCount As Integer) As Polygon
        Dim p As New Polygon

        For i As Integer = 1 To verticesCount - 1
            Dim x As Double = maxWidth * GetRandomDouble()
            Dim y As Double = maxHeight * GetRandomDouble()
            p.Vertices.Add(New Vertex(x, y))
        Next
        ClosePolygon(p)
        If Not p.IsClockwise Then
            p.Reverse()
        End If

        Return p
    End Function

    Public Sub SetPolygonSets(ByVal verticesCount As Integer)
        _PolygonSetA = New PolygonSet
        Dim polygon As Polygon = GetSelfIntersectingRandomPolygon(_SideLength,
            _SideLength, verticesCount)
        _PolygonSetA.Polygons.Add(polygon)

        _PolygonSetB = New PolygonSet
        polygon = GetSelfIntersectingRandomPolygon(_SideLength,
            _SideLength, verticesCount)
        _PolygonSetB.Polygons.Add(polygon)
    End Sub

    Public Sub TestSpeedSelfIntersecting(ByVal testObj As TestableObj,
            ByVal loops As Integer)
        Dim oA As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetA)
        Dim oB As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetB)

        Dim watch As New Stopwatch

        Dim oC As Object = Nothing
        Try
            GC.Collect()
            watch.Start()
            For i As Integer = 1 To loops
                oC = testObj.GetIntersection(oA, oB)
            Next
            watch.Stop()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return
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
        'svgWriter.StrokeWidth = _SideLength / 400
        'svgWriter.Write(psA, testObj.Id & ".SLFA")
        'svgWriter.Write(psB, testObj.Id & ".SLFB")
        'svgWriter.Write(psC, testObj.Id & ".SLFC")

        Dim sourcePolygonsCount As Integer = GetPolygonsCount(psA) +
            GetPolygonsCount(psB)
        Dim sourceVerticesCount As Integer = GetVerticesCount(psA) +
            GetVerticesCount(psB)
        Dim resultPolygonsCount As Integer = GetPolygonsCount(psC)
        Dim resultVerticesCount As Integer = GetVerticesCount(psC)
        Dim message As String =
            String.Format("TS {0,-10} ", testObj.Id) &
            String.Format("l {0:0000} ", loops) &
            String.Format("ms {0:00000000} ", watch.ElapsedMilliseconds) &
            String.Format("PS {0:00000} ", sourcePolygonsCount) &
            String.Format("VS {0:00000000} ", sourceVerticesCount) &
            String.Format("PR {0:000000} ", resultPolygonsCount) &
            String.Format("VR {0:00000000}", resultVerticesCount)
        Console.WriteLine(message)
    End Sub
End Class
