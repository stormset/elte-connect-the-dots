using System;
using System.IO;
using System.Threading.Tasks;
using Squares.Droid.Persistence;
using Squares.Persistence;
using Squares.Model;
using Xamarin.Forms;
using System.Text;
using Squares.Utilities;

[assembly: Dependency(typeof(AndroidDataAccess))]
namespace Squares.Droid.Persistence
{
    /// <summary>
    /// Game data access implementation.
    /// </summary>
    public class AndroidDataAccess : ISquaresDataAccess<PlayerType>
    {
        /// <summary>
        /// Load from file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>The restored game state.</returns>
        public async Task<Tuple<SquaresTable<PlayerType>, PlayerType>> LoadAsync(String path)
        {
            // get the path of the file
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            // a fájlmûveletet taszk segítségével végezzük (aszinkron módon)
            // String[] values = (await Task.Run(() => File.ReadAllText(filePath))).Split(' ');

            using (StreamReader reader = new StreamReader(filePath))
            {
                String line = await reader.ReadLineAsync();
                String[] numbers = line.Split(' '); // split to tokens
                Int32 rows = Int32.Parse(numbers[0]); // parse row count
                Int32 columns = Int32.Parse(numbers[1]); // parse column count
                if (rows != columns)
                    throw new InvalidOperationException();

                SquaresTable<PlayerType> table = new SquaresTable<PlayerType>(rows, columns); // create table

                String playerString = await reader.ReadLineAsync(); // current player
                PlayerType currentPlayer = (PlayerType)Enum.Parse(typeof(PlayerType), playerString, true);

                // restore edges
                String edgesString = await reader.ReadLineAsync();

                if (edgesString != null) //if there are edges
                {
                    String[] edgesStringArray = edgesString.Split(';');

                    for (int i = 0; i < edgesStringArray.Length - 1; i++) //skip the last, empty string (after last ';')
                    {
                        String[] sLabelEdge = edgesStringArray[i].Split(':');

                        PlayerType label = (PlayerType)Enum.Parse(typeof(PlayerType), sLabelEdge[0], true);

                        String[] sEdges = sLabelEdge[1].Split('-');
                        table.AddEdgeBetween(Vertex.Parse(sEdges[0]), Vertex.Parse(sEdges[1]), label);
                    }
                }

                // restore squares
                String squaresString = await reader.ReadLineAsync();
                if (squaresString != null) //if there are squares
                {
                    String[] squaresStringArray = squaresString.Split(';');

                    for (int i = 0; i < squaresStringArray.Length - 1; i++) //skip the last, empty string (after last ';')
                    {
                        String[] sLabelVertex = squaresStringArray[i].Split(':');

                        PlayerType label = (PlayerType)Enum.Parse(typeof(PlayerType), sLabelVertex[0], true);

                        table.AddSquare(Vertex.Parse(sLabelVertex[1]), label);
                    }
                }

                return new Tuple<SquaresTable<PlayerType>, PlayerType>(table, currentPlayer);
            }
        }

        /// <summary>
        /// Save to file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="table">The game data to print to file.</param>
        public async Task SaveAsync(String path, SquaresTable<PlayerType> table, PlayerType currentPlayer)
        {
            StringBuilder sb = new StringBuilder();

            using (StringWriter writer = new StringWriter(sb))
            {
                writer.WriteLine(table.Rows + " " + table.Columns); // print the size
                writer.WriteLine(currentPlayer); // print current player

                foreach (var edge in table.Edges)
                {
                    await writer.WriteAsync(edge.ToString() + ";");
                }
                await writer.WriteLineAsync();

                foreach (var square in table.Squares)
                {
                    await writer.WriteAsync(square.Value + ":" + square.Key + ";");
                }
            }

            // get the path of the file
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            // write to file
            await Task.Run(() => File.WriteAllText(filePath, sb.ToString()));
        }
    }
}