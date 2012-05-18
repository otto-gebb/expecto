﻿namespace Fuchu

module NUnitTests = 
    open Fuchu
    open Fuchu.NUnit
    open Fuchu.NUnitTestTypes
    open NUnit.Framework

    [<Tests>]
    let tests = 
        "From NUnit" =>> [
            "nothing" =>
                fun () ->
                    let test = NUnitTestToFuchu typeof<string>
                    let result = evalSilent test
                    Assert.AreEqual(0, result.Length)

            "basic" =>> [
                let test = NUnitTestToFuchu typeof<ATestFixture>
                let result = evalSilent test
                yield "read tests" =>
                    fun () ->
                        Assert.AreEqual(2, result.Length)
                        Assert.AreEqual("Fuchu.NUnitTestTypes+ATestFixture/ATest", result.[0].Name)
                        Assert.AreEqual("Fuchu.NUnitTestTypes+ATestFixture/AnotherTest", result.[1].Name)
                yield "executed tests" =>
                    fun () ->
                        Assert.True(TestResult.isPassed result.[0].Result)
                        Assert.True(TestResult.isFailed result.[1].Result)
            ]

            "with setup" =>
                fun () ->
                    let test = NUnitTestToFuchu typeof<ATestFixtureWithSetup>
                    Assert.False(ATestFixtureWithSetup.TearDownCalled, "TearDown was called")
                    let result = evalSilent test
                    Assert.AreEqual(1, result.Length)
                    Assert.True(TestResult.isPassed result.[0].Result, "Test not passed")
                    Assert.True(ATestFixtureWithSetup.TearDownCalled, "TearDown was not called")

            "with teardown and exception in test" =>
                fun () ->
                    let test = NUnitTestToFuchu typeof<ATestFixtureWithExceptionAndTeardown>
                    Assert.False(ATestFixtureWithExceptionAndTeardown.TearDownCalled, "TearDown was called")
                    let result = evalSilent test
                    Assert.AreEqual(1, result.Length)
                    Assert.True(TestResult.isFailed result.[0].Result, "Test not failed")
                    Assert.True(ATestFixtureWithExceptionAndTeardown.TearDownCalled, "TearDown was not called")
        ]
