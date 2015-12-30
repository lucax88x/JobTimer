using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Data.Access.Test
{
    [TestFixture]
    public class ShortcutTest
    {
        Fixture _fixture;
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async void shortcut_insert_and_delete()
        {
            var context = new JobTimerDbContext();
            var shortcutAccess = new ShortcutAccess(context);

            var shortcut = new Shortcut();
            shortcut.UserName = _fixture.Create<string>();
            shortcut.Shortcuts = _fixture.Create<string>();

            await shortcutAccess.SaveAsync(shortcut);

            shortcut.ID.Should().NotBe(0);
            
            var loaded = await shortcutAccess.LoadAsync(shortcut.ID);

            loaded.UserName.Should().Be(shortcut.UserName);
            loaded.Shortcuts.Should().Be(shortcut.Shortcuts);

            var deletedCount = await shortcutAccess.DeleteAsync(loaded);

            deletedCount.Should().BeGreaterOrEqualTo(1);
        }
    }
}

