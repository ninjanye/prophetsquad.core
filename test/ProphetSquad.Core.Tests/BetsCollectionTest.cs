using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class BetsCollectionTest    
    {
        [Fact]
        public void ReturnsCollectionOfBets()
        {
            var result = new BetsCollection();
            Assert.IsAssignableFrom<IEnumerable<Bet>>(result);
        }      
    }

    public class BetsCollection : IEnumerable<Bet>
    {
        public BetsCollection()
        {
        }

        public IEnumerator<Bet> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Bet
    {
        
    }
}