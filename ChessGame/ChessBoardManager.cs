using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class ChessBoardManager
    {
        private readonly Board _board;
        private bool _turn;

        public bool Turn { get => _turn; set => _turn = value; }

        private Figure _lastMoved;

       

        public ChessBoardManager(bool turn = true)
        {
            _board = new Board();
            _turn = turn;
        }

        public void ChangeTurn() 
        {
            this._turn = !_turn;
        }

        public void UpdatePosition(Figure figure, Cell cellFrom, Cell cellTo)
        {
            UpdateBoard(cellFrom, cellTo, out Figure beatenFigure);
            OnFigureMoved(figure, cellFrom, beatenFigure);
        }

        public void MoveFigure(string cellFrom, string cellTo) 
        {
            var figure = GetFigureByCell(cellFrom);
            figure.Move(new Cell(cellTo));

            UpdateBoard(new Cell(cellFrom), new Cell(cellTo), out Figure beatenFigure);
            _turn = !_turn;

            OnFigureMoved(figure, new Cell(cellFrom), beatenFigure);
        }

        public void SetTurn(bool turn)
        {
            _turn = turn;
        }

        // operations
        public void AddFigure(string cellString, string typeString, string colorString)
        {
            Type type = GetTypeFromString(typeString);
            Color color = GetCellColorFromString(colorString);
            Cell cell = new Cell(cellString);

            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            _board.Add(cell, figure);
        }

        private Type GetTypeFromString(string type)
        {
            switch (type)
            {
                case "Q": return typeof(Queen);
                case "R": return typeof(Rook);
                case "K": return typeof(King);
                case "N": return typeof(Knight);
                case "B": return typeof(Bishop);
                default: return typeof(Pawn);
            }
        }

        private Color GetCellColorFromString(string color)
        {
            return color == "B" ? Color.Black : Color.White;
        }

        public Figure GetFigureByCell(Cell cellOfFigure)
        {
            string key = cellOfFigure.ToString();
            return _board[key];
        }

        public Figure GetFigureByCell(string cellString)
        {
            Figure figure = _board[cellString];
            return figure;
        }

        public King GetTheKing(Color colorOfKing)
        {
            foreach (var item in _board)
            {
                if (item.GetType() == typeof(King) && item.Color == colorOfKing)
                {
                    return item as King;
                }
            }

            return null;
        }

        public Figure GetFigure(Type typeOfFigure, Color colorofFigure, int order = 1)
        {
            foreach (var item in _board)
            {
                if (item.GetType() == typeOfFigure && item.Color == colorofFigure)
                {
                    if (order == 1)
                    {
                        return item;
                    }
                    order--;
                }
            }
            return null;
        }

        public void UpdateBoard(Cell movedFrom, Cell movedTo, out Figure beatenFigure)
        {
            string oldCell = movedFrom.ToString();
            string newCell = movedTo.ToString();

            Figure figure = _board[oldCell];
            beatenFigure = _board[newCell];

            _board.Remove(oldCell);
            _board[newCell] = figure;

            
        }

        public void ResetBoard() 
        {
            _board.Clear();
        }

        private List<Cell> GetCheckDefenseCells(King king, Figure influencingFigure)
        {
            var checkDefenseCells = new List<Cell>
            {
                influencingFigure.CurrentCell
            };

            if (influencingFigure is Pawn || influencingFigure is Knight)
            {
                return checkDefenseCells;
            }

            var kingCell = king.CurrentCell;
            var figureCell = influencingFigure.CurrentCell;

            Cell defenseCell = new Cell(figureCell.ToString());
            if (kingCell.Letter > figureCell.Letter)
            {
                if (kingCell.Number > figureCell.Number)
                {

                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter + 1), defenseCell.Number + 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {

                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter + 1), defenseCell.Number - 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
                else
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter + 1), defenseCell.Number);
                        checkDefenseCells.Add(defenseCell);

                    }
                }
            }
            else if (kingCell.Letter < figureCell.Letter)
            {
                if (kingCell.Number > figureCell.Number)
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter - 1), defenseCell.Number + 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter - 1), defenseCell.Number - 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
                else
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell((char)(defenseCell.Letter - 1), defenseCell.Number);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
            }
            else
            {
                if (kingCell.Number > figureCell.Number)
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell(defenseCell.Letter, defenseCell.Number + 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {
                    while (defenseCell != kingCell)
                    {
                        defenseCell = new Cell(defenseCell.Letter, defenseCell.Number - 1);
                        checkDefenseCells.Add(defenseCell);
                    }
                }
            }

            return checkDefenseCells;
        }

        private int GetGameStatus()
        {
            if (IsMate())
            {
                return 2;
            }
            else if (IsStaleMate())
            {
                return 3;
            }
            else if (IsCheck())
            {
                return 1;
            }
           
            return 0;
        }

        public List<string> GetPossibleMoves(string cellString)
        {
            List<string> possibleMoves = new List<string>();
            Cell currentCell = new Cell(cellString);
            Figure figure = GetFigureByCell(currentCell);

            if (figure is King)
            {
                if (IsShortCastelingPossible(figure.Color, out Cell cellToMoveS))
                {
                    possibleMoves.Add(cellToMoveS.ToString());
                }

                if (IsLongCastelingPossible(figure.Color, out Cell cellToMoveL))
                {
                    possibleMoves.Add(cellToMoveL.ToString());
                }
            }

            if (figure is Pawn)
            {
                foreach (var pawnCell in (figure as Pawn).MovementCells)
                {
                    if (figure.CurrentCell != pawnCell && IsFreeCell(pawnCell) && 
                        IsCheckDefenseCell(figure, pawnCell) &&
                         IsNotOpeningCheck(figure, pawnCell))
                    {
                        possibleMoves.Add(pawnCell.ToString());
                    }
                }
            }

            foreach (var cell in figure.InfluencedCells)
            {
                if (IsPossibleMoveForCurrentPosition(figure, cell) && figure.CurrentCell != cell)
                {
                    possibleMoves.Add(cell.ToString());
                }
            }

            return possibleMoves;
        }

        public Dictionary<string, string> GetBoard() 
        {
            var boardDescription = new Dictionary<string, string>();
            foreach (var figure in _board)
            {
                boardDescription[figure.CurrentCell.ToString()] = figure.Name;
            }

            return boardDescription;
        }

        //validations
        public bool IsFreeCell(Cell cellTomove)
        {
            return !_board.ContainsKey(cellTomove);
        }

        public bool IsPossibleToMove(Figure figure, Cell cellToMove)
        {
            if (!(figure is Pawn) && !figure.InfluencedCells.Contains(cellToMove))
            {
                return false;
            }

            if (GetFigureByCell(cellToMove) != null && GetFigureByCell(cellToMove).Color == figure.Color)
            {
                return false;
            }

            if (figure is Pawn)
            {
                return (GetFigureByCell(cellToMove) != null) || IsPawnEnPasssentPossible(figure, cellToMove);
            }

            char letterMoveFrom = figure.CurrentCell.Letter;
            char letterMoveTo = cellToMove.Letter;

            int numberMoveFrom = figure.CurrentCell.Number;
            int numberMoveTo = cellToMove.Number;

            // Moving vertically
            if (letterMoveFrom == letterMoveTo)
            {
                Cell cell;
                if (numberMoveFrom < numberMoveTo)
                {
                    // Checks if the path from cell to move from to cell to move to is completly free
                    for (int i = numberMoveFrom + 1; i < numberMoveTo; i++)
                    {
                        cell = new Cell(figure.CurrentCell.Letter, i);

                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = numberMoveFrom - 1; i > numberMoveTo; i--)
                    {
                        cell = new Cell(figure.CurrentCell.Letter, i);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            // Moving horizonally
            else if (numberMoveFrom == numberMoveTo)
            {
                Cell cell;
                if (letterMoveFrom < letterMoveTo)
                {
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = letterMoveFrom + 1; i < letterMoveTo; i++)
                    {
                        cell = new Cell((char)i, numberMoveFrom);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = letterMoveFrom - 1; i > letterMoveTo; i--)
                    {
                        cell = new Cell((char)i, numberMoveFrom);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            //Moving by diaganal
            else
            {
                switch (numberMoveFrom > numberMoveTo)
                {
                    //Case1: up-right
                    //Case2: up-left
                    case true when letterMoveFrom > letterMoveTo:
                        {
                            // Checks if the path from cell to move from to cell to move to is completely free
                            for (int i = numberMoveFrom - 1, j = letterMoveFrom - 1; i > numberMoveTo && j > letterMoveTo; i--, j--)
                            {
                                var cell = new Cell((char)j, i);
                                if (!IsFreeCell(cell))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    //Case3: down-right
                    case true when letterMoveFrom < letterMoveTo:
                        {
                            // Checks if the path from cell to move from to cell to move to is completely free
                            for (int i = numberMoveFrom - 1, j = letterMoveFrom + 1; i > numberMoveTo && j < letterMoveTo; i--, j++)
                            {
                                var cell = new Cell((char)j, i);
                                if (!IsFreeCell(cell))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    default:
                        {
                            if (numberMoveFrom < numberMoveTo && letterMoveFrom > letterMoveTo)
                            {
                                // Checks if the path from cell to move from to cell to move to is completely free
                                for (int i = numberMoveFrom + 1, j = letterMoveFrom - 1; i < numberMoveTo && j > letterMoveTo; i++, j--)
                                {
                                    var cell = new Cell((char)j, i);
                                    if (!IsFreeCell(cell))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                            //Case4: down-left
                            else
                            {
                                // Checks if the path from cell to move from to cell to move to is completely free
                                for (int i = numberMoveFrom + 1, j = letterMoveFrom + 1; i < numberMoveTo && j < letterMoveTo; i++, j++)
                                {
                                    var cell = new Cell((char)j, i);
                                    if (!IsFreeCell(cell))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                }
            }
        }

        public bool IsUnderCheckCell(Cell cellToMove, Color colorOfPlayer, out Figure influencingFigure)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.Contains(cellToMove) && item.Color != colorOfPlayer && IsPossibleToMove(item, cellToMove))
                {
                    influencingFigure = item;
                    return true;
                }
            }

            influencingFigure = null;
            return false;
        }

        public bool IsUnderCheckCell(Cell cellToMove, Color colorOfPlayer)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.Contains(cellToMove) && item.Color != colorOfPlayer && IsPossibleToMove(item, cellToMove))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCheck(Color kingColor)
        {
            var king = GetTheKing(kingColor);
            var kingCell = king.CurrentCell;

            return IsUnderCheckCell(kingCell, kingColor);
        }

        public bool IsCheck()
        {
            var kingColor = _turn ? Color.White : Color.Black;
            var king = GetTheKing(kingColor);
            var kingCell = king.CurrentCell;

            return IsUnderCheckCell(kingCell, kingColor);
        }

        public bool IsMate()
        {

            var kingColor = _turn ? Color.White : Color.Black;
            King king = GetTheKing(kingColor);
            if (IsUnderCheckCell(king.CurrentCell, kingColor, out Figure influencingFigure))
            {
                foreach (var cell in king.InfluencedCells)
                {
                    if (cell != king.CurrentCell && ((IsFreeCell(cell) && IsNotOpeningCheck(king, cell)) ||
                       (!IsFreeCell(cell) && GetFigureByCell(cell).Color != king.Color && IsNotOpeningCheck(king, cell))))
                    {
                        return false;
                    }
                }
              
                var cells = GetCheckDefenseCells(king, influencingFigure);
                foreach (var item in _board)
                {
                    if (item.Color == kingColor && item != king) 
                    {
                        foreach (var cell in cells)
                        {
                            if (item.InfluencedCells.Contains(cell) && IsPossibleToMove(item, cell))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool IsStaleMate()
        {
            if (_board.Count == 2) 
            {
                bool twoKingsLeft = true;
                foreach (var figure in _board)
                {
                    if (!(figure is King)) 
                    {
                        twoKingsLeft = false;
                    }
                }

                if (twoKingsLeft) 
                {
                    return true;
                }
            }

            var kingColor = _turn ? Color.White : Color.Black;
            King king = GetTheKing(kingColor);

            if (!IsCheck())
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsUnderCheckCell(item, king.Color) && item != king.CurrentCell)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool IsInfluencedCell(Cell cellToCheck, Color colorOfPlayer)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.Contains(cellToCheck) && item.Color != colorOfPlayer)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPossibleMoveForCurrentPosition(Figure figureToMove, Cell cellTo)
        {
            return IsPossibleToMove(figureToMove, cellTo) &&
                   IsCheckDefenseCell(figureToMove, cellTo) &&
                   IsNotOpeningCheck(figureToMove, cellTo);
        }

        public bool IsShortCastelingPossible(Color turn, out Cell castelingKingCell)
        {
            if (IsCheck()) 
            {
                castelingKingCell = null;
                return false;
            }

            if (turn == Color.White)
            {
                if (_board["F1"] == null && !IsUnderCheckCell(new Cell("F1"), turn, out _)
                    && _board["G1"] == null && !IsUnderCheckCell(new Cell("G1"), turn, out _))
                {
                    var king = GetTheKing(turn);

                    var rook = GetFigureByCell(new Cell("H1"));

                    if (rook != null && !king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("G1");
                        return true;
                    }
                }
            }
            else
            {
                if (_board["F8"] == null && !IsUnderCheckCell(new Cell("F8"), turn, out _)
                    && _board["G8"] == null && !IsUnderCheckCell(new Cell("G8"), turn, out _))
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("H8"));

                    if (rook != null && !king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("G8");
                        return true;
                    }
                }
            }

            castelingKingCell = null;
            return false;
        }

        public bool IsLongCastelingPossible(Color turn, out Cell castelingKingCell)
        {
            if (IsCheck())
            {
                castelingKingCell = null;
                return false;
            }

            if (turn == Color.White)
            {
                if (_board["B1"] == null && !IsUnderCheckCell(new Cell("B1"), turn, out _) &&
                    _board["C1"] == null && !IsUnderCheckCell(new Cell("C1"), turn, out _) &&
                    _board["D1"] == null && !IsUnderCheckCell(new Cell("D1"), turn, out _))
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("A1"));

                    if (!king.HasMoved && rook!=null && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("C1");
                        return true;
                    }
                }
            }
            else
            {
                if (_board["B8"] == null && !IsUnderCheckCell(new Cell("B8"), turn, out _) &&
                    _board["C8"] == null && !IsUnderCheckCell(new Cell("C8"), turn, out _) &&
                    _board["D8"] == null && !IsUnderCheckCell(new Cell("D8"), turn, out _))
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("A8"));

                    if (!king.HasMoved && rook!=null && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("C8");
                        return true;
                    }
                }
            }

            castelingKingCell = null;
            return false;
        }

        public bool IsCheckDefenseCell(Figure figureToMove, Cell cellTo)
        {
            if (figureToMove is King)
            {
                return true;
            }

            var king = GetTheKing(figureToMove.Color);

            // Check if it is on cell under influence
            if (IsUnderCheckCell(king.CurrentCell, king.Color, out Figure influencingFigure))
            {
                var checkDefenseCells = GetCheckDefenseCells(king, influencingFigure);
                return checkDefenseCells.Contains(cellTo);
            }

            return true;
        }

        private bool IsNotOpeningCheck(Figure figureToMove, Cell cellTo)
        {
            var currentCell = figureToMove.CurrentCell;
            var stadingFigure = _board[cellTo.ToString()];

            figureToMove.PhantomMove(cellTo);
            UpdateBoard(currentCell, cellTo, out _);

            bool IsNotOpeningCheck = !IsCheck(figureToMove.Color);

            figureToMove.PhantomMove(currentCell);
            UpdateBoard(cellTo, currentCell, out _);
            if (stadingFigure != null)
            {
                _board[cellTo.ToString()] = stadingFigure;
            }

            return IsNotOpeningCheck;
        }

        private bool IsPawnEnPasssentPossible(Figure figureToMove, Cell cellTo)
        {
            return figureToMove is Pawn && _lastMoved is Pawn &&
                (_lastMoved as Pawn).MovedFirstTime &&
                figureToMove.CurrentCell.Number == _lastMoved.CurrentCell.Number &&
                Math.Abs(figureToMove.CurrentCell.Letter - _lastMoved.CurrentCell.Letter) == 1 && cellTo.Letter == _lastMoved.CurrentCell.Letter;
        }

        private bool WasPawnEnPassentPossible(Figure figureToMove, Cell cellFrom, Cell cellTo) 
        {
            return figureToMove is Pawn && _lastMoved is Pawn &&
                (_lastMoved as Pawn).MovedFirstTime &&
                cellFrom.Number == _lastMoved.CurrentCell.Number &&
                Math.Abs(cellFrom.Letter - _lastMoved.CurrentCell.Letter) == 1 && cellTo.Letter == _lastMoved.CurrentCell.Letter;
        }


        public event EventHandler<FigureMoveEventArgs> FigureMoved;

        protected virtual void OnFigureMoved(Figure movedFigure, Cell cellFrom, Figure beatenFigure)
        {
            
            int status = GetGameStatus();
            bool? winner = null;
            if (status == 2) 
            {
                winner = !_turn;
            }

            IsCastelingDone(movedFigure, cellFrom, out Cell rookCellFrom, out Cell rookCellTo);
            string CastelingCellFrom = rookCellFrom == null ? string.Empty : rookCellFrom.ToString();
            string CastelingCellTo = rookCellTo == null ? string.Empty : rookCellTo.ToString();

            IsPawnEnPassentDone(movedFigure, cellFrom, out Figure beatenPawn);

            if (beatenFigure == null && beatenPawn != null) 
            {
                beatenFigure = beatenPawn;
            }
            

            FigureMoved?.Invoke(this, new FigureMoveEventArgs
            {
                MovedFigureName = movedFigure.Name,
                BeatenFigureName = beatenFigure == null ? string.Empty : beatenFigure.Name,
                BeatenFigureCell = beatenFigure == null ? string.Empty : beatenFigure.CurrentCell.ToString(),
                CellFrom = cellFrom.ToString(),
                CellTo = movedFigure.CurrentCell.ToString(),
                CastelingCellFrom = CastelingCellFrom,
                CastelingCellTo = CastelingCellTo,
                GameStatus = status,
                CurrentPlayer = _turn,
                WinnerPlayer = winner
            }) ;

            _lastMoved = movedFigure;
        }

        private bool IsCastelingDone(Figure movedFigure, Cell cellFrom, out Cell rookCellFrom, out Cell rookCellTo) 
        {
            if (movedFigure is King) 
            {
                if (movedFigure.Color == Color.Black && cellFrom.ToString() == "E8")
                {
                    if (movedFigure.CurrentCell.ToString() == "G8")
                    {
                        var rook = GetFigureByCell(new Cell("H8"));
                        rookCellFrom = rook.CurrentCell;
                        rookCellTo = new Cell("F8");
                        rook.Move(rookCellTo);
                        UpdateBoard(rookCellFrom, rookCellTo, out _);

                        return true;
                    }

                    if (movedFigure.CurrentCell.ToString() == "C8")
                    {
                        var rook = GetFigureByCell(new Cell("A8"));
                        rookCellFrom = rook.CurrentCell;
                        rookCellTo = new Cell("D8");
                        rook.Move(rookCellTo);
                        UpdateBoard(rookCellFrom, rookCellTo, out _);

                        return true;
                    }
                }
                else if (movedFigure.Color == Color.White && cellFrom.ToString() == "E1") 
                {
                    if (movedFigure.CurrentCell.ToString() == "G1")
                    {
                        var rook = GetFigureByCell(new Cell("H1"));
                        rookCellFrom = rook.CurrentCell;
                        rookCellTo = new Cell("F1");
                        rook.Move(rookCellTo);
                        UpdateBoard(rookCellFrom, rookCellTo, out _);
                        
                        return true;
                    }

                    if (movedFigure.CurrentCell.ToString() == "C1")
                    {
                        var rook = GetFigureByCell(new Cell("A1"));
                        rookCellFrom = rook.CurrentCell;
                        rookCellTo = new Cell("D1");
                        rook.Move(rookCellTo);
                        UpdateBoard(rookCellFrom, rookCellTo, out _);

                        return true;
                    }
                }
            }

            rookCellFrom = null;
            rookCellTo = null;

            return false;
        }
        private bool IsPawnEnPassentDone(Figure movedFigure, Cell cellFrom, out Figure beatenPawn) 
        {
            if (WasPawnEnPassentPossible(movedFigure, cellFrom, movedFigure.CurrentCell)) 
            {
                beatenPawn = _lastMoved;
                return true;
            }

            beatenPawn = null;
            return false;
        }
    }
}