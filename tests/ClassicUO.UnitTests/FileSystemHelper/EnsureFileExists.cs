﻿using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ClassicUO.UnitTests.FileSystemHelper
{
    public class EnsureFileExists
    {
        [Fact]
        public void EnsureFileExists_For_InvalidPath_Should_ThrowException()
        {
            Action act = () => Utility.FileSystemHelper.EnsureFileExists("abc\\invalid_file\\name.extension");

            act.Should().Throw<FileNotFoundException>();
        }
    }
}