
enum LEVEL
{
    LEVEL_0,
    LEVEL_1,
    LEVEL_2,
    _END_
}

enum PLAYER_STATE
{ 
    IDLE,
    JUMP,
    SUPER_JUMP,
    SHIELD,
    HIT,
    ATTACK,
    JUMP_ATTACK,
    DEAD,
    _END_
}

enum TAGS
{ 
    MainCamera,
    Player,
    Ground,
    Object,
    Attack,
    _END_
}

enum GO_POOL_TYPE
{ 
    OBJECT_0,
    OBJECT_1,
    OBJECT_2,
    PROJECTILE_0,
    PROJECTILE_1,
    PROJECTILE_2,
    EXPLOSION,
    _END_
}


enum UI_POOL_TYPE
{
    COMMBO,
    SCORE,
    AIR_SHOT,
    _END_
}
