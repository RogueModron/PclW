Imports System
Imports System.Math
Imports PclWCommon
Imports PclWTestCommon


Module CorrectnessTests
    Private Const PIH As Double = PI / 2
    Private Const PIQ As Double = PI / 4


    Public Sub TestDifference(ByVal testObj As TestableObj)
        Console.WriteLine("TestDifference {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(5, 300, 0, -PIH))
        svgWriter.Write(ps, testObj.Id & ".D01A")
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(49, 330, 100, PIH))
        svgWriter.Write(ps, testObj.Id & ".D01B")
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = testObj.GetDifference(o01A, o01B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
        svgWriter.Write(ps, testObj.Id & ".D01C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".D02A")
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 200, 190))
        ps.Polygons.Add(GetTestClockwisePolygon(50, 150, 100))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D02B")
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o02C As Object = testObj.GetDifference(o02A, o02B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
        svgWriter.Write(ps, testObj.Id & ".D02C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".D03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(25, 220, 180, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(25, 170, 100, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D03B")
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o03C As Object = testObj.GetDifference(o03A, o03B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
        svgWriter.Write(ps, testObj.Id & ".D03C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D04A")
        Dim o04A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D04B")
        Dim o04B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o04C As Object = testObj.GetDifference(o04A, o04B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o04C)
        svgWriter.Write(ps, testObj.Id & ".D04C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(37, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(37, 250, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D05A")
        Dim o05A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(37, 300, 250, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(37, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".D05B")
        Dim o05B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o05C As Object = testObj.GetDifference(o05A, o05B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05C)
        svgWriter.Write(ps, testObj.Id & ".D05C")
        Dim o05D As Object = testObj.GetDifference(o05A, o05C)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05D)
        svgWriter.Write(ps, testObj.Id & ".D05D")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))
    End Sub

    Public Sub TestIntersection(ByVal testObj As TestableObj)
        Console.WriteLine("TestIntersection {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(7, 300, 0, 5, -PIH))
        svgWriter.Write(ps, testObj.Id & ".I01A")
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(49, 330, 100, PIH))
        svgWriter.Write(ps, testObj.Id & ".I01B")
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = testObj.GetIntersection(o01A, o01B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
        svgWriter.Write(ps, testObj.Id & ".I01C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".I02A")
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 200, 190))
        ps.Polygons.Add(GetTestClockwisePolygon(50, 150, 100))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".I02B")
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o02C As Object = testObj.GetIntersection(o02A, o02B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
        svgWriter.Write(ps, testObj.Id & ".I02C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".I03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(25, 220, 180, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(25, 170, 100, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".I03B")
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o03C As Object = testObj.GetIntersection(o03A, o03B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
        svgWriter.Write(ps, testObj.Id & ".I03C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".I04A")
        Dim o04A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".I04B")
        Dim o04B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o04C As Object = testObj.GetIntersection(o04A, o04B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o04C)
        svgWriter.Write(ps, testObj.Id & ".I04C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(40, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(40, 250, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".I05A")
        Dim o05A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(40, 300, 250, -PIH))
        svgWriter.Write(ps, testObj.Id & ".I05B")
        Dim o05B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o05C As Object = testObj.GetIntersection(o05A, o05B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05C)
        svgWriter.Write(ps, testObj.Id & ".I05C")
        Dim o05D As Object = testObj.GetIntersection(o05A, o05C)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05D)
        svgWriter.Write(ps, testObj.Id & ".I05D")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))
    End Sub

    Public Sub TestUnion(ByVal testObj As TestableObj)
        Console.WriteLine("TestUnion {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 250, 0, -PIH))
        svgWriter.Write(ps, testObj.Id & ".U01A")
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(49, 300, 100, PIH))
        svgWriter.Write(ps, testObj.Id & ".U01B")
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = testObj.GetUnion(o01A, o01B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
        svgWriter.Write(ps, testObj.Id & ".U01C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(5, 300, 0, PIH))
        svgWriter.Write(ps, testObj.Id & ".U02A")
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 225, 0))
        ps.Polygons.Add(GetTestClockwisePolygon(5, 150, 0, -PIH))
        ReverseLastPolygon(ps)
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        svgWriter.Write(ps, testObj.Id & ".U02B")
        Dim o02C As Object = testObj.GetUnion(o02A, o02B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
        svgWriter.Write(ps, testObj.Id & ".U02C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, PIH))
        svgWriter.Write(ps, testObj.Id & ".U03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 250, 0))
        ps.Polygons.Add(GetTestClockwisePolygon(5, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        svgWriter.Write(ps, testObj.Id & ".U03B")
        Dim o03C As Object = testObj.GetUnion(o03A, o03B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
        svgWriter.Write(ps, testObj.Id & ".U03C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".U04A")
        Dim o04A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".U04B")
        Dim o04B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o04C As Object = testObj.GetUnion(o04A, o04B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o04C)
        svgWriter.Write(ps, testObj.Id & ".U04C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 0))
        ps.Polygons.Add(GetTestClockwisePolygon(50, 225, 150))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".U05A")
        Dim o05A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 225, 150))
        svgWriter.Write(ps, testObj.Id & ".U05B")
        Dim o05B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o05C As Object = testObj.GetUnion(o05A, o05B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05C)
        svgWriter.Write(ps, testObj.Id & ".U05C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))
    End Sub

    Public Sub TestXor(ByVal testObj As TestableObj)
        Console.WriteLine("TestXor {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".X01A")
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 150))
        svgWriter.Write(ps, testObj.Id & ".X01B")
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = testObj.GetXor(o01A, o01B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
        svgWriter.Write(ps, testObj.Id & ".X01C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".X02A")
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 200, 190))
        ps.Polygons.Add(GetTestClockwisePolygon(50, 150, 100))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".X02B")
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o02C As Object = testObj.GetXor(o02A, o02B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
        svgWriter.Write(ps, testObj.Id & ".X02C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".X03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(25, 220, 180, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(25, 170, 100, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".X03B")
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o03C As Object = testObj.GetXor(o03A, o03B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
        svgWriter.Write(ps, testObj.Id & ".X03C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".X04A")
        Dim o04A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(3, 300, 0, -PIH))
        ps.Polygons.Add(GetTestClockwisePolygon(3, 200, 0, -PIH))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".X04B")
        Dim o04B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o04C As Object = testObj.GetXor(o04A, o04B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o04C)
        svgWriter.Write(ps, testObj.Id & ".X04C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 200))
        svgWriter.Write(ps, testObj.Id & ".X05A")
        Dim o05A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".X05B")
        Dim o05B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o05C As Object = testObj.GetXor(o05A, o05B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05C)
        svgWriter.Write(ps, testObj.Id & ".X05C")
        Dim o05D As Object = testObj.GetXor(o05A, o05C)
        ps = testObj.GetAdaptedOutputToPolygonSet(o05D)
        svgWriter.Write(ps, testObj.Id & ".X05D")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))
    End Sub

    Public Sub TestMixed(ByVal testObj As TestableObj)
        Console.WriteLine("TestMixed {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".M01A")
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 301, 250))
        svgWriter.Write(ps, testObj.Id & ".M01B")
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = testObj.GetIntersection(o01A, o01B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
        svgWriter.Write(ps, testObj.Id & ".M01C")
        Dim o01D As Object = testObj.GetDifference(o01A, o01C)
        ps = testObj.GetAdaptedOutputToPolygonSet(o01D)
        svgWriter.Write(ps, testObj.Id & ".M01D")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 300, 0))
        svgWriter.Write(ps, testObj.Id & ".M02A")
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(50, 301, 250))
        svgWriter.Write(ps, testObj.Id & ".M02B")
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o02C As Object = testObj.GetDifference(o02A, o02B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
        svgWriter.Write(ps, testObj.Id & ".M02C")
        Dim o02D As Object = testObj.GetDifference(o02A, o02C)
        ps = testObj.GetAdaptedOutputToPolygonSet(o02D)
        svgWriter.Write(ps, testObj.Id & ".M02D")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 100, 0, PIQ))
        ps.Polygons.Add(GetTestClockwisePolygon(4, 100, 0, PIQ, 0, 150))
        svgWriter.Write(ps, testObj.Id & ".M03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 40, 0, PIQ))
        ps.Polygons.Add(GetTestClockwisePolygon(4, 20, 0, PIQ))
        ReverseLastPolygon(ps)
        svgWriter.Write(ps, testObj.Id & ".M03B")
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o03C As Object = testObj.GetDifference(o03A, o03B)
        ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
        svgWriter.Write(ps, testObj.Id & ".M03C")

        Console.WriteLine("P: {0:000}; V: {1:000}",
            GetPolygonsCount(ps), GetVerticesCount(ps))
   End Sub

    Public Sub TestExceptions(ByVal testObj As TestableObj)
        Console.WriteLine("TestExceptions {0}", testObj.Id)

        Dim svgWriter As New SvgWriter

        Dim ps As New PolygonSet
        ps.Polygons.Add(New Polygon)
        Dim o01A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(New Polygon)
        Dim o01B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o01C As Object = Nothing
        Select Case testObj.Id
            Case "Geos"
                '
            Case Else
                testObj.GetIntersection(o01A, o01B)
        End Select
        If o01C Is Nothing Then
            Console.WriteLine("P: ---; V: ---")
        Else
            ps = testObj.GetAdaptedOutputToPolygonSet(o01C)
            Console.WriteLine("P: {0:000}; V: {1:000}",
                GetPolygonsCount(ps), GetVerticesCount(ps))
        End If

        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(5, 300, 0))
        Dim o02A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(5, 300, 0, 0, 0, 1000))
        Dim o02B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o02C As Object = testObj.GetIntersection(o02A, o02B)
        If o02C Is Nothing Then
            Console.WriteLine("P: ---; V: ---")
        Else
            ps = testObj.GetAdaptedOutputToPolygonSet(o02C)
            Console.WriteLine("P: {0:000}; V: {1:000}",
                GetPolygonsCount(ps), GetVerticesCount(ps))
        End If

        Dim coordinates() As Double = {
            -150, -150,
            150, 150,
            -150, 150,
            150, -150,
            -150, -150
            }
        ps = New PolygonSet
        ps.Polygons.Add(New Polygon(coordinates))
        svgWriter.Write(ps, testObj.Id & ".E03A")
        Dim o03A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        ps = New PolygonSet
        ps.Polygons.Add(GetTestClockwisePolygon(4, 200, 0))
        svgWriter.Write(ps, testObj.Id & ".E03B")
        Dim o03B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o03C As Object = Nothing
        Select Case testObj.Id
            Case "Cgal", "Geos", "Ggl", "MSSqlST"
                '
            Case Else
                o03C = testObj.GetIntersection(o03A, o03B)
        End Select
        If o03C Is Nothing Then
            Console.WriteLine("P: ---; V: ---")
        Else
            ps = testObj.GetAdaptedOutputToPolygonSet(o03C)
            svgWriter.Write(ps, testObj.Id & ".E03C")
            Console.WriteLine("P: {0:000}; V: {1:000}",
                GetPolygonsCount(ps), GetVerticesCount(ps))
        End If

        coordinates = {
            -300, -300,
            -300, 300,
            300, 300,
            300, 0,
            150, 300,
            0, 0,
            -150, 300,
            -300, 0,
            -150, -300,
            0, 0,
            150, -300,
            300, 0,
            300, -300,
            -300, -300
            }
        ps = New PolygonSet
        ps.Polygons.Add(New Polygon(coordinates))
        svgWriter.Write(ps, testObj.Id & ".E04A")
        Dim o04A As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        coordinates = {
            -300, 0,
            -150, 300,
            0, 0,
            150, 300,
            300, 0,
            150, -300,
            0, 0,
            -150, -300,
            -300, 0
            }
        ps = New PolygonSet
        ps.Polygons.Add(New Polygon(coordinates))
        svgWriter.Write(ps, testObj.Id & ".E04B")
        Dim o04B As Object = testObj.GetAdaptedInputFromPolygonSet(ps)
        Dim o04C As Object = Nothing
        Select Case testObj.Id
            Case "Cgal", "MSSqlST"
                '
            Case Else
                o04C = testObj.GetUnion(o04A, o04B)
        End Select
        If o04C Is Nothing Then
            Console.WriteLine("P: ---; V: ---")
        Else
            ps = testObj.GetAdaptedOutputToPolygonSet(o04C)
            svgWriter.Write(ps, testObj.Id & ".E04C")
            Console.WriteLine("P: {0:000}; V: {1:000}",
                GetPolygonsCount(ps), GetVerticesCount(ps))
        End If
    End Sub
End Module
