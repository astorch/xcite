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
            subset.AddListener(collectionListener);
            
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

        [Test]
        public void ModifyAndUpdate() {
            // Arrange
            ObservableCollection<int> set = new ObservableCollection<int>(new[] {2, 3, 5, 6, 7});
            IObservableCollectionSubset<int> subSet = set.Where(i => i % 2 == 0);

            CollectionListener collectionListener = new CollectionListener();
            subSet.AddListener(collectionListener);

            int[] array1 = subSet.ToArray();

            // Act
            subSet.Remove(6);
            int[] array2 = subSet.ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] {2, 6}, array1);
            CollectionAssert.AreEqual(new[] {2}, array2);
            Assert.AreEqual(1, collectionListener.Removed);
        }

        [Test]
        public void ModifyOnlySubset() {
            // Arrange
            ObservableCollection<int> set = new ObservableCollection<int>(new[] { 2, 3, 5, 6, 7 });
            IObservableCollectionSubset<int> subSet = set.Where(i => i % 2 == 0);

            CollectionListener collectionListener = new CollectionListener();
            subSet.AddListener(collectionListener);

            int[] array1 = subSet.ToArray();

            // Act
            subSet.Remove(6, false);
            int[] array2 = subSet.ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] { 2, 6 }, array1);
            CollectionAssert.AreEqual(new[] { 2, 6 }, array2);
            Assert.AreEqual(1, collectionListener.Removed);
        }

        class CollectionListener : IEnumerableListener<int> {

            public int Added { get; private set; }

            public int Removed { get; private set; } 

            /// <inheritdoc />
            public void OnItemAdded(IObservableEnumerable<int> itemCollection, int item) {
                Added++;
            }

            /// <inheritdoc />
            public void OnItemRemoved(IObservableEnumerable<int> itemCollection, int item) {
                Removed++;
            }

            /// <inheritdoc />
            public void OnCleared(IObservableEnumerable<int> itemCollection) {
                
            }
        }
    }
}