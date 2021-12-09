using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Piece nextPiece { get; private set; }

    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector3Int previewPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public int Score;
    public Text TXTPuntaje;

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        this.nextPiece = this.gameObject.AddComponent<Piece>();
        this.nextPiece.enabled = false;

        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SetNextPiece();
        SpawnPiece();
    }

    private void SetNextPiece()
    {
        if (this.nextPiece.cells != null) {
            Clear(this.nextPiece);
        }

        // Número Aleatorio
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];

        this.nextPiece.Initialize(this, this.previewPosition, data);
        Set(this.nextPiece);
    }

    public void SpawnPiece()
    {

        // Instanciar Pieza
        this.activePiece.Initialize(this, this.spawnPosition, this.nextPiece.data);

        if (!IsValidPosition(this.activePiece, this.spawnPosition)) {
            GameOver();
        } else {
            Set(this.activePiece);
        }

        SetNextPiece();
    }

    private void GameOver(){
        this.tilemap.ClearAllTiles();
        this.Score = 0;
        this.TXTPuntaje.text = "Score: " + 0;

        //Pantalla de GameOver
    }


    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    // LIMPIAR
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    // LA POSICION ES VALIDA?
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        // Posicion valida si todas las celdas son validas
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // Si un bloque de un tetrimino esta fuera de los limites, no es valida la posicion
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // Si una pieza ya ocupa ese lugar, no es valida la posicion
            if (this.tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        // Ciclo para limpiar Filas (de abajo hacia arriba)
        while (row < bounds.yMax)
        {
            // Recorrer Filas de arriba
            if (IsLineFull(row)) {
                LineClear(row);
                SumScore();
            } else {
                row++;
            }
        }
    }

    // FILA LLENA
    public bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // Si la Fila no esta completa (faltan bloques), no es valida la limpieza
            if (!this.tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    // LIMPIAR FILAS
    public void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        // Limpia todos los bloques de la fila
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        // Baja las demas filas
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void SumScore(){
        this.Score += 10;
        this.TXTPuntaje.text = "Score: " + this.Score;
        Debug.Log(this.Score);
    }

}
