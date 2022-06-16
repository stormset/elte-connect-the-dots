using System;
using System.Threading.Tasks;
using System.Linq;
using Squares.Model;
using Squares.Persistence;
using Squares.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Squares.Test
{
    [TestClass]
    public class SquaresTest
    {
        private SquaresGameModel _model; // the model to test
        private SquaresTable<PlayerType> _mockedTable; // mocked game table
        private Mock<ISquaresDataAccess<PlayerType>> _mock; // mocked data access layer

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new SquaresTable<PlayerType>(); // 5x5
            _mockedTable.AddEdgeBetween(VertexOf(0, 0), VertexOf(1, 0), PlayerType.Red);
            _mockedTable.AddEdgeBetween(VertexOf(1, 0), VertexOf(1, 1), PlayerType.Blue);
            _mockedTable.AddEdgeBetween(VertexOf(1, 1), VertexOf(0, 1), PlayerType.Red);
            _mockedTable.AddEdgeBetween(VertexOf(0, 1), VertexOf(0, 0), PlayerType.Red); //Square of red;
            _mockedTable.AddSquare(VertexOf(0, 0), PlayerType.Red); // red: 1, blue: 0 points
            // define a game table to be able to mock the persistence layer

            _mock = new Mock<ISquaresDataAccess<PlayerType>>();

            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(new Tuple<SquaresTable<PlayerType>, PlayerType>(_mockedTable, PlayerType.Red))); //Current player Red
            // return the _mockedTable and default(PlayerType) when LoadAsync is called

            _model = new SquaresGameModel(_mock.Object);
            // create model with the mocked ISquaresDataAccess

            _model.GameAdvanced += new EventHandler<SquaresEventArgs>(Model_GameAdvanced);

            _model.GameOver += new EventHandler<GameOverEventArgs>(Model_GameOver);
        }


        [TestMethod]
        public void SquaresGameModelNewGameTest()
        {
            _model.TableSize = 3;
            _model.NewGame();

            Assert.AreEqual(3, _model.Table.Rows); // size is correct
            Assert.AreEqual(_model.Table.Rows, _model.Table.Columns); // size is correct

            Assert.AreEqual(0, _model.Table.Edges.Count()); // no edges
            Assert.AreEqual(0, _model.Table.Squares.Count()); // no sqares
        }

        [TestMethod]
        public void SquaresGameModelEdgeSelectedTest()
        {
            _model.TableSize = 5;
            _model.NewGame();

            _model.EdgeSelected(VertexOf(0, 0), VertexOf(1, 0));
            Assert.IsTrue(_model.Table.HasEdgeBetween(VertexOf(0, 0), VertexOf(1, 0)));
        }

        [TestMethod]
        public void SquaresGameModelEdgeSelectedNewSquareTest()
        {
            _model.TableSize = 5;
            _model.NewGame();

            //create a square; top-left corner: 0,0
            PlayerType current = _model.CurrentPlayer;
            _model.EdgeSelected(VertexOf(0, 0), VertexOf(1, 0));
            _model.EdgeSelected(VertexOf(1, 0), VertexOf(1, 1));
            _model.EdgeSelected(VertexOf(1, 1), VertexOf(0, 1));
            _model.EdgeSelected(VertexOf(0, 1), VertexOf(0, 0));

            if(current == PlayerType.Red)
                Assert.IsTrue(_model.Table.Squares.Any(s => s.Key.Equals(VertexOf(0, 0)) && s.Value == PlayerType.Blue)); // square of blue was added
            else
                Assert.IsTrue(_model.Table.Squares.Any(s => s.Key.Equals(VertexOf(0, 0)) && s.Value == PlayerType.Red)); // square of red was added
        }

        [TestMethod]
        public void SquaresGameModelPlayerSteppingTest()
        {
            _model.TableSize = 5;
            _model.NewGame();

            PlayerType current = _model.CurrentPlayer;

            _model.EdgeSelected(VertexOf(0, 0), VertexOf(1, 0));
            if (current == PlayerType.Red)
                Assert.IsTrue(_model.CurrentPlayer == PlayerType.Blue); // next player is blue
            else
                Assert.IsTrue(_model.CurrentPlayer == PlayerType.Red); // next player is red

            _model.EdgeSelected(VertexOf(1, 0), VertexOf(1, 1));
            _model.EdgeSelected(VertexOf(1, 1), VertexOf(0, 1));
            _model.EdgeSelected(VertexOf(0, 1), VertexOf(0, 0));

            if (current == PlayerType.Red)
                Assert.IsTrue(_model.CurrentPlayer == PlayerType.Blue); // square of blue was added, he is the next again
            else
                Assert.IsTrue(_model.CurrentPlayer == PlayerType.Red); // square of red was added, he is the next again
        }

        [TestMethod]
        public void SquaresGameModelScoreOfTest()
        {
            _model.TableSize = 5;
            _model.NewGame();

            PlayerType current = _model.CurrentPlayer;

            _model.EdgeSelected(VertexOf(0, 0), VertexOf(1, 0));
            _model.EdgeSelected(VertexOf(1, 0), VertexOf(1, 1));
            _model.EdgeSelected(VertexOf(1, 1), VertexOf(0, 1));
            _model.EdgeSelected(VertexOf(0, 1), VertexOf(0, 0));

            if (current == PlayerType.Red)
            {
                Assert.IsTrue(_model.ScoreOf(PlayerType.Blue) == 1); // square of blue was added, point goes to him
                Assert.IsTrue(_model.ScoreOf(PlayerType.Red) == 0);
            }
            else
            {
                Assert.IsTrue(_model.ScoreOf(PlayerType.Red) == 1); // square of red was added, point goes to him
                Assert.IsTrue(_model.ScoreOf(PlayerType.Blue) == 0);
            }
        }

        [TestMethod]
        public async Task SudokuGameModelLoadTest()
        {
            _model.NewGame();

            // load game
            await _model.LoadGameAsync(String.Empty);

            Assert.IsTrue(_model.CurrentPlayer == PlayerType.Red); // current player is correct
            // scores are correct
            Assert.AreEqual(1, _model.ScoreOf(PlayerType.Red));
            Assert.AreEqual(0, _model.ScoreOf(PlayerType.Blue));
            // table is restored correctly
            _mockedTable.HasEdgeBetween(VertexOf(0, 0), VertexOf(1, 0));
            _mockedTable.HasEdgeBetween(VertexOf(1, 0), VertexOf(1, 1));
            _mockedTable.HasEdgeBetween(VertexOf(1, 1), VertexOf(0, 1));
            _mockedTable.HasEdgeBetween(VertexOf(0, 1), VertexOf(0, 0));

            // check that LoadAsync was called
            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        private void Model_GameAdvanced(Object sender, SquaresEventArgs e)
        {
            Assert.IsTrue(_model.Table.HasEdgeBetween(e.Vertex1, e.Vertex2)); // edge is added
        }

        private void Model_GameOver(Object sender, GameOverEventArgs e)
        {
            Assert.IsTrue(_model.IsGameOver); // game is surely over

            if (e.Winner != null) // Winner has the max points
            {
                Int32 maxScore = 0;
                foreach (PlayerType player in Enum.GetValues(typeof(PlayerType)))
                {
                    Int32 score = _model.ScoreOf(player);
                    if (score > maxScore)
                    {
                        maxScore = score;
                    }
                }

                Assert.IsTrue(_model.ScoreOf((PlayerType)e.Winner) == maxScore);
            }
        }

        private static Vertex VertexOf(Int32 x, Int32 y)
        {
            return new Vertex(x, y);
        }
    }
}
