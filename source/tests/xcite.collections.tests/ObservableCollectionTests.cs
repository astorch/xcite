using NUnit.Framework;

namespace xcite.collections.tests {
    [TestFixture]
    public class ObservableCollectionTests {
        [Test]
        public void WhereIteration() {
            // Arrange
            ObservableCollection<int> set = new ObservableCollection<int>(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
            IObservableEnumerable<int> subset = set.Where(i => i % 2 == 0);

            CollectionListener collectionListener = new CollectionListener();
            subset.AddCollectionListener(collectionListener);
            
            int[] array1 = subset.ToArray();

            // Act
            set.Add(12);
            set.Add(13);
            set.Add(14);
            int[] array2 = subset.ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] {2, 4, 6, 8}, array1);
            CollectionAssert.AreEqual(new[] {2, 4, 6, 8, 12, 14}, array2);
            Assert.AreEqual(3, collectionListener.Added);
        }

        class CollectionListener : ICollectionListener<int> {

            public int Added { get; private set; } = 0;

            /// <inheritdoc />
            public void OnItemAdded(IObservableEnumerable<int> itemCollection, int item) {
                Added++;
            }

            /// <inheritdoc />
            public void OnItemRemoved(IObservableEnumerable<int> itemCollection, int item) {
                
            }

            /// <inheritdoc />
            public void OnCleared(IObservableEnumerable<int> itemCollection) {
                
            }
        }
    }
}