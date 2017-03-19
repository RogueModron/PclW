Imports PclWCommon
Imports PclWTestCommon

Class BenchClassic
    'NOTE: 
    '- Polygons used to test PolyBoolean required.
    '- Polygons are scaled down for PolyBoolean.c20 limits.

    Private _PolygonSetA As PolygonSet
    Private _PolygonSetB As PolygonSet

    Public Function LoadWlrPolygonSet(ByVal fileFullName As String,
            ByVal scale As Double
            ) As PolygonSet
        Try
            Dim reader As New IO.StreamReader(fileFullName)
            Dim line As String = reader.ReadLine
            If Integer.Parse(line) <> 1 Then
                Return Nothing
            End If
            line = reader.ReadLine
            Dim polygonCount As Integer = Integer.Parse(line)
            Dim polygonSet As New PolygonSet
            Do While polygonCount > 0
                line = reader.ReadLine
                Dim verticesCount As Integer = Integer.Parse(line)
                Dim polygon As New Polygon
                Do While verticesCount > 0
                    line = reader.ReadLine
                    Dim parts() As String = line.Split(",")
                    Dim vertex As New Vertex(Integer.Parse(parts(0)) * scale,
                        Integer.Parse(parts(1)) * scale)
                    polygon.Vertices.Add(vertex)
                    verticesCount = verticesCount - 1
                Loop
                polygonSet.Polygons.Add(polygon)
                If polygonSet.Polygons.Count = 1 Then
                    If Not polygon.IsClockwise Then
                        polygon.Reverse()
                    End If
                Else
                    If polygon.IsClockwise Then
                        polygon.Reverse()
                    End If
                End If
                polygonCount = polygonCount - 1
            Loop
            Return polygonSet
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub SetPolygonSets(Optional ByVal scale As Double = 1)
        Static lastScale As Double = Double.NaN
        If lastScale = scale _
                AndAlso _PolygonSetA IsNot Nothing _
                AndAlso _PolygonSetB IsNot Nothing Then
            Return
        End If
        lastScale = scale

        _PolygonSetA = LoadWlrPolygonSet(
            IO.Path.Combine(Environment.CurrentDirectory, "s.wlr"), scale)
        _PolygonSetB = LoadWlrPolygonSet(
            IO.Path.Combine(Environment.CurrentDirectory, "c.wlr"), scale)
    End Sub

    Public Sub TestSpeedClassic(ByVal testObj As TestableObj,
            ByVal loops As Integer, ByVal scale As Double)
        SetPolygonSets(scale)
        If _PolygonSetA Is Nothing OrElse _PolygonSetB Is Nothing Then
            Return
        End If

        Dim oA As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetA)
        Dim oB As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetB)

        Dim watch As New Stopwatch

        Dim oC As Object = Nothing
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            'Threading.Thread.Sleep(1000)
            watch.Start()
            For k As Integer = 1 To loops
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
        'svgWriter.StrokeWidth = 20
        'svgWriter.Write(psA, testObj.Id & ".CLSA")
        'svgWriter.Write(psB, testObj.Id & ".CLSB")
        'svgWriter.Write(psC, testObj.Id & ".CLSC")

        Dim sourcePolygonsCount As Integer = GetPolygonsCount(psA) +
            GetPolygonsCount(psB)
        Dim sourceVerticesCount As Integer = GetVerticesCount(psA) +
            GetVerticesCount(psB)
        Dim resultPolygonsCount As Integer = GetPolygonsCount(psC)
        Dim resultVerticesCount As Integer = GetVerticesCount(psC)
        Dim message As String =
            String.Format("TC {0,-10} ", testObj.Id) &
            String.Format("l {0:0000} ", loops) &
            String.Format("ms {0:00000000} ", watch.ElapsedMilliseconds) &
            String.Format("PS {0:00000} ", sourcePolygonsCount) &
            String.Format("VS {0:00000000} ", sourceVerticesCount) &
            String.Format("PR {0:000000} ", resultPolygonsCount) &
            String.Format("VR {0:00000000}", resultVerticesCount)
        Console.WriteLine(message)
    End Sub
End Class
