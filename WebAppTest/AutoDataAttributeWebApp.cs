using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppTest
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AutoDataAttributeWebApp : AutoDataAttribute
    {
        public AutoDataAttributeWebApp() : base(() => CreateCustomFixture())
        {
 
        }

        private static IFixture CreateCustomFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
