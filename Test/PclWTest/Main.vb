Imports System.Reflection
Imports PclWTestCommon

Module Main
    Private _objectsToTest As Dictionary(Of String, TestableObj)

    Sub Main()
        _objectsToTest = New Dictionary(Of String, TestableObj)

        LoadObjectToTest("Bop", "PclWBop.dll", "PclWBop.BopWrapper",
                    False,
                    New InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter,
                    New InputAdapterFromRegionToRegionWithAnticlockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)
        LoadObjectToTest("Cgal", "PclWCgal.dll", "PclWCgal.CgalWrapper",
                    False,
                    New InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter,
                    New InputAdapterFromRegionToRegionWithAnticlockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)
        LoadObjectToTest("Clipper", "PclWClipper.dll", "PclWClipper.ClipperWrapper",
                    False,
                    New InputAdapterFromPolygonSetToPolygonSetWithImplicitHoles,
                    New InputAdapterFromRegionToPolygonSetWithImplicitHoles,
                    New OutputAdapterFromPolygonSetToPolygonSet)
        LoadObjectToTest("Geos", "PclWGeos.dll", "PclWGeos.GeosWrapper",
                    True,
                    New InputAdapterFromPolygonSetToRegionWithClockwiseOuter,
                    New InputAdapterFromRegionToRegionWithClockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)
        LoadObjectToTest("Ggl", "PclWGgl.dll", "PclWGgl.GglWrapper",
                    True,
                    New InputAdapterFromPolygonSetToRegionWithClockwiseOuter,
                    New InputAdapterFromRegionToRegionWithClockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)
        LoadObjectToTest("Gpc", "PclWGpc.dll", "PclWGpc.GpcWrapper",
                    False,
                    New InputAdapterFromPolygonSetToPolygonSetWithExplicitHoles,
                    New InputAdapterFromRegionToPolygonSetWithExplicitHoles,
                    New OutputAdapterFromPolygonSetToPolygonSet)
        LoadObjectToTest("Gtl", "PclWGtl.dll", "PclWGtl.GtlWrapper",
                    False,
                    New InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter,
                    New InputAdapterFromRegionToRegionWithAnticlockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)
        LoadObjectToTest("KBool", "PclWKBool.dll", "PclWKBool.KBoolWrapper",
                    False,
                    New InputAdapterFromPolygonSetToPolygonSetWithImplicitHoles,
                    New InputAdapterFromRegionToPolygonSetWithImplicitHoles,
                    New OutputAdapterFromPolygonSetToPolygonSet)
        LoadObjectToTest("TerraLib", "PclWTerraLib.dll", "PclWTerraLib.TerraLibWrapper",
                    True,
                    New InputAdapterFromPolygonSetToRegionWithClockwiseOuter,
                    New InputAdapterFromRegionToRegionWithClockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)

        LoadObjectToTest("PolyB", "WrapperPolyBoolean.dll", "WrapperPolyBoolean.PolyBooleanWrapper",
                    False,
                    New InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter,
                    New InputAdapterFromRegionToRegionWithAnticlockwiseOuter,
                    New OutputAdapterFromRegionToPolygonSet)

        LoadObjectToTest("MSSqlST", "WrapperMSSqlServerTypes.dll", "WrapperMSSqlServerTypes.MSSqlServerTypesWrapper")


        'LoadObjectToTest("GpcPIW", "WrapperGpcPInvoke.dll", "WrapperGpcPInvoke.GpcPInvokeWrapper")
        'LoadObjectToTest("ClipperCS", "WrapperClipperCSharp.dll", "WrapperClipperCSharp.ClipperCSharpWrapper")


        'Console.WriteLine("Press a key.")
        'Console.ReadKey()


        'TestCorrectness()


        ExecuteBenchClassic()

        'ExecuteBenchKnown(2, 0)
        'ExecuteBenchKnown(5, 0)

        'ExecuteBenchRandom()

        'ExecuteBenchGrid()
        'ExecuteBenchGridCostant()


        'ExecuteBenchSelfIntersection()


        'TestGpcCppCliVsPI()
        'TestClipperCppCliVsCs()


        'TestMemory()
    End Sub

    Private Sub LoadObjectToTest(ByVal id As String,
            ByVal assemblyName As String,
            ByVal completeTypeName As String,
            ByVal closedPolygonsRequired As Boolean,
            ByVal inputAdapterFromPolygonSet As Object,
            ByVal inputAdapterFromRegion As Object,
            ByVal outputAdapter As Object)
        Dim testableAssembly As Assembly = Assembly.LoadFrom(assemblyName)
        Dim typeInstance As Object = testableAssembly.CreateInstance(completeTypeName)
        Dim tmp As New TestablePclW(id, typeInstance, closedPolygonsRequired,
            inputAdapterFromPolygonSet, inputAdapterFromRegion, outputAdapter)
        _objectsToTest.Add(id, tmp)
    End Sub

    Private Sub LoadObjectToTest(ByVal id As String,
            ByVal assemblyName As String,
            ByVal completeTypeName As String)
        Dim testableAssembly As Assembly = Assembly.LoadFrom(assemblyName)
        Dim typeInstance As Object = testableAssembly.CreateInstance(completeTypeName)
        typeInstance.Id = id
        _objectsToTest.Add(id, typeInstance)
    End Sub


    Private Sub ExecuteBenchClassic()
        Const TestLoops As Integer = 7
        Const TestLoopsCgal As Integer = 2
        Const TestScale As Double = 1
        Const TestScalePolyBoolean As Double = 0.125

        'NOTE: Wlr files required.
        Console.WriteLine("Test Classic ...")
        Dim classicTest As New BenchClassic
        'First test for warmup.
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            Dim scale As Double = TestScale
            Select Case element.Key
                Case "PolyB"
                    scale = TestScalePolyBoolean
                Case Else
                    scale = TestScale
            End Select
            classicTest.TestSpeedClassic(element.Value, 1, scale)
        Next
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            'NOTE: Cgal il too slow but this test test must be passed so the number of loops is reduces.
            'The average is the most important value.
            Dim loops As Double = TestLoops
            Select Case element.Key
                Case "Cgal"
                    loops = TestLoopsCgal
                Case Else
                    loops = TestLoops
            End Select
            'NOTE: PolyB is limited to 20 bit values so the polygon must be scaled. 
            'The results do not differ significatively between the two scales.
            Dim scale As Double = TestScale
            Select Case element.Key
                Case "PolyB"
                    scale = TestScalePolyBoolean
                Case Else
                    scale = TestScale
            End Select
            classicTest.TestSpeedClassic(element.Value, loops, scale)
        Next
        Console.WriteLine("")
    End Sub

    Private Sub ExecuteBenchGrid()
        Const TestLoops As Integer = 7

        Console.WriteLine("Test Grid ...")
        Dim gridTest As New BenchGrid
        gridTest.HolesForRow = 5
        gridTest.Mode = BenchGrid.ModeType.IntersectionOn
        gridTest.SideLengthOuter = 2500
        For h As Integer = 1 To 50
            Console.WriteLine("-- {0}", h)
            gridTest.SetPolygonSets()
            For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
                'NOTE: Libraries too slow.
                Select Case element.Key
                    Case "Cgal", "Geos", "Ggl"
                        Continue For
                End Select
                gridTest.TestSpeedGrid(element.Value, TestLoops - Math.Floor(h / 20))
            Next
            gridTest.HolesForRow = gridTest.HolesForRow + 5
        Next
        Console.WriteLine("")
    End Sub

    Private Sub ExecuteBenchGridCostant()
        Const TestLoops As Integer = 5

        Console.WriteLine("Test Grid Constant ...")
        Dim gridTest As New BenchGrid
        gridTest.HolesForRow = 100
        gridTest.SideLengthOuter = 2500
        For h As Integer = 0 To 10
            Console.WriteLine("-- {0}", h)
            gridTest.SetPolygonSets(h * 10)
            For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
                'NOTE: Libraries too slow.
                Select Case element.Key
                    Case "Cgal" ', "Geos", "Ggl"
                        'Continue For
                End Select
                gridTest.TestSpeedGrid(element.Value, TestLoops)
            Next
        Next
        Console.WriteLine("")
    End Sub

    Private Sub ExecuteBenchKnown(ByVal setNumber As Integer, ByVal angleOffset As Double)
        Console.WriteLine("Test Known ...")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            'First test for warmup.
            Dim tests() As Integer
            If setNumber >= 1 Then
                tests = {
                    8, 8, 24, 40, 56, 72, 88, 104, 120
                    }
                For Each test As Integer In tests
                    TestSpeedKnown(element.Value, test, 50, angleOffset)
                Next
                'NOTE: Bop has problems with the rest of this test.
                Select Case element.Key
                    Case "Bop"
                        Continue For
                End Select
            End If
            If setNumber >= 2 Then
                tests = {
                    240, 360, 480, 600, 720, 840, 960, 1080, 1200
                    }
                For Each test As Integer In tests
                    TestSpeedKnown(element.Value, test, 15, angleOffset)
                Next
            End If
            If setNumber >= 3 Then
                tests = {
                    2400, 3600, 4800, 6000, 7200, 8400, 9600, 10800, 12000
                    }
                For Each test As Integer In tests
                    TestSpeedKnown(element.Value, test, 5, angleOffset)
                Next
            End If
            If setNumber >= 4 Then
                tests = {
                    24000, 48000, 72000, 96000, 120000
                    }
                For Each test As Integer In tests
                    TestSpeedKnown(element.Value, test, 2, angleOffset)
                Next
            End If

            'NOTE: For X64 builds this case could give weird execution times.
            'TestSpeedKnown1(element.Value, 72000, 1, 0)
            'For j = 1 To 20
            '    TestSpeedKnown1(element.Value, 48000, 1, 0)
            'Next

            If setNumber >= 5 Then
                'NOTE: Cgal is too slow.
                Select Case element.Key
                    Case "Cgal"
                        Continue For
                End Select
                tests = {
                    240000, 480000, 720000
                    }
                For Each test As Integer In tests
                    TestSpeedKnown(element.Value, test, 1, angleOffset)
                Next
            End If
        Next
        Console.WriteLine("")
    End Sub

    Private Sub ExecuteBenchRandom()
        'NOTE: Gtl required to create random polygons, it gives "cleaner" polygons.
        '
        'Some polygons (self touching for example) could be invalid for Bop, PolyBoolean, TerraLib.
        'Cgal is really susceptible and slow at the same time, so it is excluded from this test.

        If Not _objectsToTest.ContainsKey("Gtl") Then
            Return
        End If

        Const TestLoops As Integer = 7

        Console.WriteLine("Test Random ...")
        Dim randomTest As New BenchRandom
        randomTest.BuildAdding = False
        randomTest.Generator = _objectsToTest("Gtl")
        randomTest.MaxRandomPolygonSideLenght = 50
        randomTest.SideLength = 2500
        randomTest.RandomPolygons = 10
        randomTest.UseRectangles = False
        For h As Integer = 1 To 50
            Console.WriteLine("-- {0}", h)
            randomTest.SetRegions()
            For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
                Select Case element.Key
                    Case "Cgal"
                        Continue For
                End Select
                randomTest.TestSpeedRandom(element.Value, TestLoops - Math.Floor(h / 20))
            Next
            randomTest.RandomPolygons = randomTest.RandomPolygons + 2
        Next
        Console.WriteLine("")
    End Sub

    Private Sub ExecuteBenchSelfIntersection()
        Const TestLoops As Integer = 10
        Const TestSelfVerticesIncrement As Integer = 10

        Console.WriteLine("Test Self-Intersecting ...")
        Dim selfTest As New BenchSelfIntersecting
        selfTest.SideLength = 2500
        For h As Integer = 1 To 10
            Console.WriteLine("-- {0}", h)
            selfTest.SetPolygonSets(h * TestSelfVerticesIncrement)
            For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
                Select Case element.Value.Id
                    Case "Bop", "Clipper", "Gpc", "Gtl", "KBool"
                        'NOTE: Only Gpc and Clipper give comparable results.
                    Case Else
                        Continue For
                End Select
                selfTest.TestSpeedSelfIntersecting(element.Value, TestLoops - Math.Floor(h / 20))
            Next
        Next
        Console.WriteLine("")
    End Sub

    Private Sub TestCorrectness()
        Console.WriteLine("Correctness Tests ...")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestDifference(element.Value)
        Next
        Console.WriteLine("")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestIntersection(element.Value)
        Next
        Console.WriteLine("")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestUnion(element.Value)
        Next
        Console.WriteLine("")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestXor(element.Value)
        Next
        Console.WriteLine("")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestMixed(element.Value)
        Next
        Console.WriteLine("")
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            TestExceptions(element.Value)
        Next
        Console.WriteLine("")
    End Sub

    Private Sub TestGpcCppCliVsPI()
        If Not (_objectsToTest.ContainsKey("Gpc") AndAlso _objectsToTest.ContainsKey("GpcPIW")) Then
            Return
        End If

        Dim objectsToTest As New List(Of TestableObj)
        objectsToTest.Add(_objectsToTest("Gpc"))
        objectsToTest.Add(_objectsToTest("GpcPIW"))

        Const TestLoops1 As Integer = 5000
        Const TestLoops2 As Integer = 10

        Console.WriteLine("Test Gpc C# VS C++/Cli ...")
        'First test for warmup.
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 8, 1, 0)
        Next
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 120, TestLoops1, 0)
            TestSpeedKnown(element, 720, TestLoops1, 0)
        Next
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 6000, TestLoops2, 0)
            TestSpeedKnown(element, 60000, TestLoops2, 0)
        Next
        Console.WriteLine("")
    End Sub

    Private Sub TestClipperCppCliVsCs()
        If Not (_objectsToTest.ContainsKey("Clipper") AndAlso _objectsToTest.ContainsKey("ClipperCS")) Then
            Return
        End If

        Dim objectsToTest As New List(Of TestableObj)
        objectsToTest.Add(_objectsToTest("Clipper"))
        objectsToTest.Add(_objectsToTest("ClipperCS"))

        Const TestLoops1 As Integer = 5000
        Const TestLoops2 As Integer = 10

        Console.WriteLine("Test Clipper C# VS C++/Cli ...")
        'First test for warmup.
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 8, 1, 0)
        Next
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 120, TestLoops1, 0)
            TestSpeedKnown(element, 720, TestLoops1, 0)
        Next
        For Each element As TestableObj In objectsToTest
            TestSpeedKnown(element, 6000, TestLoops2, 0)
            TestSpeedKnown(element, 60000, TestLoops2, 0)
        Next
        Console.WriteLine("")
    End Sub

    Private Sub TestMemory()
        ReadMemory()

        Dim callBack As Threading.TimerCallback = AddressOf ReadMemory

        Const TestScale As Double = 1
        Const TestScalePolyBoolean As Double = 0.125

        'NOTE: Wlr files required.
        Dim classicTest As New BenchClassic
        'First test for warmup.
        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            Dim scale As Double = TestScale
            Select Case element.Key
                Case "PolyB"
                    scale = TestScalePolyBoolean
                Case Else
                    scale = TestScale
            End Select
            classicTest.TestSpeedClassic(element.Value, 1, scale)
        Next

        Console.WriteLine("-- START --")

        For Each element As KeyValuePair(Of String, TestableObj) In _objectsToTest
            'NOTE: PolyB is limited to 20 bit values so the polygon must be scaled. 
            'The results do not differ significatively between the two scales.
            Dim scale As Double = TestScale
            Select Case element.Key
                Case "PolyB"
                    scale = TestScalePolyBoolean
                Case Else
                    scale = TestScale
            End Select

            Dim period As Integer = 10
            Select Case element.Key
                Case "Cgal"
                    period = 1000
            End Select

            Threading.Thread.Sleep(5000)

            Dim stateTimer As Threading.Timer = New Threading.Timer(callBack, Nothing, 1, period)
            classicTest.TestSpeedClassic(element.Value, 1, scale)
            stateTimer.Dispose()
        Next
    End Sub
End Module