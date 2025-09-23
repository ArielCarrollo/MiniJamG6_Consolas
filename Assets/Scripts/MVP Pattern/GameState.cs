public class GameState
{
    public float Coraje { get; set; }
    public int ProgresoNarrativo { get; set; }

    public GameState()
    {
        Coraje = 0.0f; // El coraje empieza bajo
        ProgresoNarrativo = 0;
    }
}