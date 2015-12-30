using System;
using Autofac;
using FluentAssertions;
using JobTimer.Utils.Security;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Utils.Test
{
    public class HashHelperTest
    {
        private IHashHelper _sut;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {            
            _sut = new HashHelper();
            _fixture = new Fixture();
        }
        
        [Test]
        public void should_hash_and_verify()
        {
            var pw = _fixture.Create<string>();
            var hashed = _sut.Hash(pw);
            hashed.Should().NotBeNullOrEmpty();
            hashed.Should().NotBe(pw);

            _sut.Verify(hashed, pw).Should().BeTrue();
        }        
    }
}
