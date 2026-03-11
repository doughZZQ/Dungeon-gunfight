
public enum Orientation
{
    north, south, west, east, none
}

public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
}

public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}
