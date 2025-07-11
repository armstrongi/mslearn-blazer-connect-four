namespace ConnectFour;

public class GameState
{
	public const int BoardCellCount = 42;
	public const int BoardRowCount = 6;
	public const int BoardColCount = 7;
	static GameState()
	{
		CalculateWinningPlaces();
	}

	/// <summary>
	/// Indicate whether a player has won, the game is a tie, or game in ongoing
	/// </summary>
	public enum WinState
	{
		No_Winner = 0,
		Player1_Wins = 1,
		Player2_Wins = 2,
		Tie = 3
	}
	private int[] _winningPieces = [];
	public int[] WinningPieces
	{
		get => _winningPieces;
		set
		{
			if (value == null || value.Length != 4)
				throw new ArgumentException("Winning pieces must be an array of 4 integers.");
			_winningPieces = value;
		}
	}
	private int _startingPlayer = 1; // 1 or 2

	public int StartingPlayer
	{
		get => _startingPlayer;
		set => _startingPlayer = value == 1 ? 1 : 2;
	}

	/// <summary>
	/// The player whose turn it is.  By default, player 1 starts first
	/// </summary>
	public int PlayerTurn => (TheBoard.Count(x => x != 0) + (_startingPlayer - 1)) % 2 + 1;
	public void AlternateStartingPlayer()
	{
		_startingPlayer = _startingPlayer == 1 ? 2 : 1;
	}

	/// <summary>
	/// Number of turns completed and pieces played so far in the game
	/// </summary>
	public int CurrentTurn { get { return TheBoard.Count(x => x != 0); } }

	public static readonly List<int[]> WinningPlaces = new();
	
	/// <summary>
	/// Populates the WinningPlaces list with all possible winning combinations on the Connect Four board.
	/// This includes every set of four consecutive positions horizontally, vertically, and diagonally (both directions).
	/// These precomputed scenarios are used to efficiently check for a win during gameplay.
	public static void CalculateWinningPlaces()
	{

		// Horizontal rows
		for (byte row = 0; row < BoardRowCount; row++)
		{
			byte rowCol1 = (byte)(row * BoardColCount);
			byte rowColEnd = (byte)((row + 1) * BoardColCount - 1);
			
			for (byte checkCol = rowCol1; checkCol <= rowColEnd - 3; checkCol++)
			{
				WinningPlaces.Add(new int[] {
					checkCol,
					(byte)(checkCol + 1),
					(byte)(checkCol + 2),
					(byte)(checkCol + 3)
					});				
			}
		}

		// Vertical Columns
		for (byte col = 0; col < BoardColCount; col++)
		{
			byte colRow1 = col;
			byte colRowEnd = (byte)(35 + col);
			
			for (byte checkRow = colRow1; checkRow <= 14 + col; checkRow += BoardColCount)
			{
				WinningPlaces.Add(new int[] {
					checkRow,
					(byte)(checkRow + BoardColCount),
					(byte)(checkRow + BoardColCount * 2),
					(byte)(checkRow + BoardColCount * 3)
					});				
			}
		}

		// forward slash diagonal "/"
		for (byte col = 0; col < 4; col++)
		{
			// starting column must be 0-3
			byte colRow1 = (byte)(21 + col);
			byte colRowEnd = (byte)(35 + col);

			for (byte checkPos = colRow1; checkPos <= colRowEnd; checkPos += BoardColCount)
			{
				WinningPlaces.Add(new int[] {
					checkPos,
					(byte)(checkPos - BoardRowCount),
					(byte)(checkPos - BoardRowCount * 2),
					(byte)(checkPos - BoardRowCount * 3)
					});

			}
		}

		// back slash diaganol "\"
		for (byte col = 0; col < 4; col++)
		{
			// starting column must be 0-3
			byte colRow1 = (byte)(0 + col);
			byte colRowEnd = (byte)(14 + col);
			
			for (byte checkPos = colRow1; checkPos <= colRowEnd; checkPos += BoardColCount)
			{
				WinningPlaces.Add(new int[] {
					checkPos,
					(byte)(checkPos + BoardRowCount + 2),
					(byte)(checkPos + (BoardRowCount + 2) * 2),
					(byte)(checkPos + (BoardRowCount + 2) * 3)
					});				
			}
		}
	}

	/// <summary>
	/// Check the state of the board for a winning scenario
	/// </summary>
	/// <returns>0 - no winner, 1 - player 1 wins, 2 - player 2 wins, 3 - draw</returns>
	public WinState CheckForWin()
	{

		// Exit immediately if less than 7 pieces are played
		if (TheBoard.Count(x => x != 0) < 7) return WinState.No_Winner;

		foreach (var scenario in WinningPlaces)
		{

			if (TheBoard[scenario[0]] == 0) continue;

			if (TheBoard[scenario[0]] ==
				TheBoard[scenario[1]] &&
				TheBoard[scenario[1]] ==
				TheBoard[scenario[2]] &&
				TheBoard[scenario[2]] ==
				TheBoard[scenario[3]])
			{
				_winningPieces = scenario;
				return (WinState)TheBoard[scenario[0]];
			}

		}

		if (TheBoard.Count(x => x != 0) == BoardCellCount) return WinState.Tie;

		return WinState.No_Winner;

	}

	/// <summary>
	/// Takes the current turn and places a piece in the 0-indexed column requested
	/// </summary>
	/// <param name="column">0-indexed column to place the piece into</param>
	/// <returns>The final array index where the piece resides</returns>
	public byte PlayPiece(int column)
	{

		// Check for a current win
		if (CheckForWin() != 0) throw new ArgumentException("Game is over");

		// Check the column
		if (TheBoard[column] != 0) throw new ArgumentException("Column is full");

		// Drop the piece in
		var landingSpot = column;
		for (var i=column;i<BoardCellCount;i+=BoardColCount)
		{
			if (TheBoard[landingSpot + BoardColCount] != 0) break;
			landingSpot = i;
		}

		TheBoard[landingSpot] = PlayerTurn;

		return ConvertLandingSpotToRow(landingSpot);

	}

	public List<int> TheBoard { get; private set; } = new List<int>(new int[BoardCellCount]);
	
	private string _player1Color = string.Empty;
	private string _player2Color = string.Empty;

	/// <summary>
	/// Gets or sets the color for Player 1 as a CSS/HTML color string.
	/// </summary>
	public string Player1Color
	{
		get => _player1Color;
		set => _player1Color = value;
	}

	/// <summary>
	/// Gets or sets the color for Player 2 as a CSS/HTML color string.
	/// </summary>
	public string Player2Color
	{
		get => _player2Color;
		set => _player2Color = value;
	}

	private int _player1Wins = 0;
	private int _player2Wins = 0;
	private int _ties = 0;

	/// <summary>
	/// Gets or sets the number of games won by Player 1.
	/// </summary>
	public int Player1Wins
	{
		get => _player1Wins;
		set => _player1Wins = value;
	}

	/// <summary>
	/// Gets or sets the number of games won by Player 2.
	/// </summary>
	public int Player2Wins
	{
		get => _player2Wins;
		set => _player2Wins = value;
	}

	/// <summary>
	/// Gets or sets the number of tied games.
	/// </summary>
	public int Ties
	{
		get => _ties;
		set => _ties = value;
	}

	public void ResetBoard()
	{
		TheBoard = new List<int>(new int[BoardCellCount]);
		_winningPieces = [];
	}

	private byte ConvertLandingSpotToRow(int landingSpot)
	{

		return (byte)(Math.Floor(landingSpot / (decimal)BoardColCount) + 1);

	}

}