# TheNewFrontier
3D Platformer Refactored Code

Este es un proyecto viejo creado originalmente sin patrones de programación, como se puede ver en el primer commit, en el cual viene el proyecto original. El objetivo de este repositorio es mejorar la estructura de los scripts, usando patrones de programación y principios SOLID para tener un código más limpio, legible y expandible. De la misma manera fue creado usando como referencia otro repositorio de mi propiedad, creado para la materia de Patrones De Diseño de tercer trimestre de la carrera de Programación De Videojuegos en SAE Institute.

Dejo el link a dicho repositorio como referencia: https://github.com/darwok/PatronesDeDise-oCPP.

## Índice
- [Diagrama UML – Singleton & MVC (Audio / Options Menu)](#diagrama-uml--singleton--mvc-audio--options-menu)
- [Diagrama UML – Command + FSM (Player)](#diagrama-uml--command--fsm-player)
- [Diagrama UML – Object Pool & Armas (Bow / Arrow / TeslaGun)](#diagrama-uml--object-pool--armas-bow--arrow--teslagun)
- [Diagrama UML – Factory (EnemySpawner / EnemyFactory)](#diagrama-uml--factory-enemyspawner--enemyfactory)
- [Diagrama UML – Prototype & Keys (Llaves / Puertas / NPC)](#diagrama-uml--prototype--keys-llaves--puertas--npc)
- [Diagrama UML – Observer & Player Stats (Health / Score / Pickups)](#diagrama-uml--observer--player-stats-health--score--pickups)
- [Diagrama general de relaciones entre patrones](#diagrama-general-de-relaciones-entre-patrones)

## Diagrama UML – Singleton & MVC (Audio / Options Menu)
```mermaid
classDiagram
class AudioManager {
  <<singleton>>
  -float masterVolume
  +float MasterVolume
  +static AudioManager Instance
  +void Awake()
  +void SetMasterVolume(float volume)
  -void ApplyVolume()
}

class OptionsModel {
  -float volume
  -float brightness
  +void Load()
  +void Save()
}

class OptionsView {
  -Slider volumeSlider
  -Slider brightnessSlider
  +Slider VolumeSlider
  +Slider BrightnessSlider
  +void Bind(float volume, float brightness)
}

class OptionsController {
  -OptionsView view
  -CanvasGroup brightnessOverlay
  -OptionsModel model
  +void Awake()
  +void OnDestroy()
  -void OnVolumeChanged(float value)
  -void OnBrightnessChanged(float value)
  -void ApplyVolume(float value)
  -void ApplyBrightness(float value)
  +void CloseOptions()
}

class MainMenuManager {
  -GameObject mainMenuPanel
  -GameObject optionsPanel
  +void Start()
  +void PlayGame()
  +void OpenOptions()
  +void CloseOptions()
  +void QuitGame()
  +void MainMenu()
}

OptionsController --> OptionsModel : usa
OptionsController --> OptionsView : controla
OptionsController --> AudioManager : ajusta volumen
OptionsController --> CanvasGroup : ajusta brillo
MainMenuManager --> OptionsController : activa/desactiva panel
MainMenuManager --> GameObject : paneles UI
```
## Diagrama UML – Command + FSM (Player)
```mermaid
classDiagram
class ICommand {
  <<interface>>
  +Execute()
}

class MoveCommand {
  -PlayerController player
  -InputAction moveAction
  -InputAction sprintAction
  +MoveCommand(PlayerController, InputActionReference, InputActionReference)
  +Execute()
}

class JumpCommand {
  -PlayerController player
  -InputAction jumpAction
  +JumpCommand(PlayerController, InputActionReference)
  +Execute()
}

class DashCommand {
  -PlayerController player
  -InputAction dashAction
  +DashCommand(PlayerController, InputActionReference)
  +Execute()
}

class AttackCommand {
  -PlayerController player
  -InputAction attackAction
  +AttackCommand(PlayerController, InputActionReference)
  +Execute()
}

class PlayerState {
  <<enumeration>>
  Normal
  Hurt
  Dashing
  Dead
}

class PlayerController {
  -Bow bow
  -TeslaGun teslaGun
  -GameObject[] weapons
  -GameObject[] ammoUI
  -Animator anim
  -float moveSpeed
  -float sprintSpeed
  -float gravity
  -float jumpHeight
  -int maxJumps
  -float dashSpeed
  -float dashDuration
  -float dashCooldown
  -float hitDamage
  -ParticleSystem hitParticles
  -AudioSource hitSound
  -Transform teleportDestination
  -InputActionReference move
  -InputActionReference jump
  -InputActionReference sprint
  -InputActionReference dash
  -InputActionReference attack
  -CharacterController controller
  -Transform cameraTransform
  -Vector3 velocity
  -int jumpCount
  -float lastDashTime
  -bool isGrounded
  -Vector3 moveDirection
  -bool isSprinting
  -PlayerState state
  -MoveCommand moveCommand
  -JumpCommand jumpCommand
  -DashCommand dashCommand
  -AttackCommand attackCommand
  -PlayerStatsSubject stats
  -KeyInventory keyInventory
  +int playerhp
  +void Awake()
  +void OnEnable()
  +void OnDisable()
  +void Start()
  +void Update()
  +void HandleMoveInput(Vector2 input, bool sprinting)
  +void HandleJumpInput()
  +void HandleDashInput()
  +void HandleAttackInput()
  +void TakeDamage()
  +void TakeDamage(float amount)
  +void ActivateWeapon(int index)
  +void SwitchWeapon(int index)
  +void TeleportTo(Transform destination)
}

ICommand <|.. MoveCommand
ICommand <|.. JumpCommand
ICommand <|.. DashCommand
ICommand <|.. AttackCommand

PlayerController --> MoveCommand : invoca
PlayerController --> JumpCommand : invoca
PlayerController --> DashCommand : invoca
PlayerController --> AttackCommand : invoca
PlayerController --> Bow : usa
PlayerController --> TeslaGun : usa
PlayerController --> PlayerStatsSubject : consulta/daño
PlayerController --> KeyInventory : consulta llaves
PlayerController --> PlayerState : usa
```
## Diagrama UML – Object Pool & Armas (Bow / Arrow / TeslaGun)
```mermaid
classDiagram
class ObjectPool {
  -GameObject prefab
  -int initialSize
  -bool expandable
  -Queue<GameObject> pool
  +void Awake()
  -GameObject CreateObject()
  +GameObject Get()
  +void Return(GameObject obj)
}

class Arrow {
  -float speed
  -float maxTime
  -float currentTime
  -Rigidbody rb
  -ObjectPool pool
  +void Init(ObjectPool pool)
  +void Awake()
  +void OnEnable()
  +void FixedUpdate()
  +void OnCollisionEnter(Collision collision)
  -void Despawn()
}

class Bow {
  +string weaponName
  -ObjectPool arrowPool
  -Transform muzzle
  -int maxAmmo
  -int currAmmo
  -int maxMags
  -int currMag
  -TextMeshProUGUI ammoText
  -TextMeshProUGUI magsText
  -float shootCooldown
  -ParticleSystem shootParticles
  -Animator playerAnimator
  -NPC npc
  -float lastShotTime
  -bool isShooting
  +void Start()
  -void UpdateUI()
  +void TryShoot()
  -IEnumerator ShootRoutine()
  +void RestockAmmo()
  +void Reload()
}

class TeslaGun {
  +string weaponName
  -LineRenderer lineRenderer
  -Transform muzzle
  -int maxAmmoTime
  -int currAmmoTime
  -int maxMag
  -int currMag
  -float shootDistance
  -LayerMask shootMask
  -ParticleSystem shootParticles
  -Animator playerAnimator
  -NPC npc
  -bool isShooting
  +void Start()
  +void TryShoot()
  -IEnumerator ShootLaserRoutine()
  +void RestockAmmo()
  +void Reload()
}

ObjectPool o--> Arrow : administra
Bow --> ObjectPool : usa para flechas
Bow --> Arrow : instancia desde el pool
TeslaGun --> NPC : consulta rango diálogo
PlayerController --> Bow : ataca
PlayerController --> TeslaGun : ataca
```
## Diagrama UML – Factory (EnemySpawner / EnemyFactory)
```mermaid
classDiagram
class EnemyFactory {
  -GameObject meleeEnemyPrefab
  -GameObject rangedEnemyPrefab
  +GameObject CreateEnemy(EnemyType type, Vector3 position, Quaternion rotation, Transform parent)
}

class EnemySpawner {
  -EnemyFactory factory
  -EnemyType enemyType
  -Transform[] spawnPoints
  -int enemiesPerPoint
  +void Start()
  +void SpawnAll()
}

class EnemyController {
  -float health
  -float speed
  -int points
  +void Start()
  +void Update()
  +void GetHit(float damage)
}

class EnemyType {
  <<enumeration>>
  Melee
  Ranged
}

EnemySpawner --> EnemyFactory : usa
EnemyFactory --> EnemyController : crea instancias
EnemyFactory --> EnemyType : tipo de enemigo
EnemySpawner --> EnemyType : configuración
```
## Diagrama UML – Prototype & Keys (Llaves / Puertas / NPC)
```mermaid
classDiagram
class KeyPrototype {
  <<prototype>>
  +string id
  +string displayName
  +Sprite icon
}

class KeyInventory {
  -HashSet<string> keys
  +void AddKey(KeyPrototype prototype)
  +bool HasKey(KeyPrototype prototype)
}

class KeyPickup {
  +KeyPrototype keyPrototype
  +void OnTriggerEnter(Collider other)
}

class KeyGiverNPC {
  +KeyPrototype requiredKey
  +KeyPrototype rewardKey
  +void TryGiveKey(KeyInventory inventory)
}

class DoorController {
  +Transform player
  +KeyPrototype requiredKey
  +float detectionRange
  +Animator animator
  -bool isOpen
  +void Update()
  -void OpenDoor()
  -void CloseDoor()
}

class NPC {
  +Transform player
  +float interactionDistance
  +GameObject interact
  +GameObject options
  +Button keyButton
  +Button byeButton
  +TextMeshProUGUI feedbackText
  +float feedbackDuration
  +KeyGiverNPC keyGiver
  +bool playerInRange
  +void Start()
  +void Update()
  -void OpenOptions()
  -void CloseDialogue()
  -void EnableCursor()
  -void DisableCursor()
  -void OnAskForKey()
  -void OnSayBye()
  -void ShowFeedback(string text)
  -IEnumerator ClearFeedbackRoutine()
  +void OnDrawGizmosSelected()
}

KeyPickup --> KeyPrototype : referencia
KeyPickup --> KeyInventory : agrega llave

KeyInventory --> KeyPrototype : consulta por id

KeyGiverNPC --> KeyPrototype : requiredKey, rewardKey
KeyGiverNPC --> KeyInventory : entrega llave

NPC --> KeyGiverNPC : delega lógica de llaves
NPC --> KeyInventory : a través del Player

DoorController --> KeyPrototype : llave requerida
DoorController --> KeyInventory : verifica llave
DoorController --> Animator : abre/cierra puerta
```
## Diagrama UML – Observer & Player Stats (Health / Score / Pickups)
```mermaid
classDiagram
class IPlayerStatsObserver {
  <<interface>>
  +OnHealthChanged(float current, float max)
  +OnScoreChanged(int score)
  +OnAmmoChanged(int ammo)
}

class PlayerStatsSubject {
  +float maxHealth
  +float CurrentHealth
  +int Score
  +int Ammo
  -List<IPlayerStatsObserver> observers
  +void Awake()
  +void RegisterObserver(IPlayerStatsObserver observer)
  +void UnregisterObserver(IPlayerStatsObserver observer)
  -void NotifyHealth()
  -void NotifyScore()
  -void NotifyAmmo()
  +void TakeDamage(float amount)
  +void Heal(float amount)
  +void AddScore(int amount)
  +void AddAmmo(int amount)
}

class HealthUI {
  -PlayerStatsSubject subject
  -Slider hpSlider
  +void OnEnable()
  +void OnDisable()
  +void OnHealthChanged(float current, float max)
  +void OnScoreChanged(int score)
  +void OnAmmoChanged(int ammo)
}

class scoreUI {
  <<singleton>>
  +static scoreUI instance
  -PlayerStatsSubject subject
  -TextMeshProUGUI scoreValue
  -TextMeshProUGUI hScoreValue
  -int highScore
  +void Awake()
  +void OnEnable()
  +void OnDisable()
  +void OnHealthChanged(float current, float max)
  +void OnScoreChanged(int score)
  +void OnAmmoChanged(int ammo)
  +void UpdateScore(int score)
  +void UpdateHighScore(int highScore)
}

class AmmoMags {
  -Vector3 rotationSpeed
  -float floatAmplitude
  -float floatFrequency
  -int ammoAmount
  -Vector3 startPosition
  +void Start()
  +void Update()
  +void OnTriggerEnter(Collider other)
}

class healScript {
  -float floatAmplitude
  -float floatFrequency
  -float healAmount
  -Vector3 startPosition
  +void Start()
  +void Update()
  +void OnTriggerEnter(Collider other)
}

PlayerStatsSubject o--> IPlayerStatsObserver : notifica

HealthUI ..|> IPlayerStatsObserver
scoreUI ..|> IPlayerStatsObserver

HealthUI --> PlayerStatsSubject : se registra
scoreUI --> PlayerStatsSubject : se registra

AmmoMags --> PlayerStatsSubject : AddAmmo()
AmmoMags --> Bow : RestockAmmo()
AmmoMags --> TeslaGun : RestockAmmo()

healScript --> PlayerStatsSubject : Heal()
```
## Diagrama general de relaciones entre patrones
```mermaid
classDiagram
AudioManager <.. OptionsController
OptionsController ..> OptionsModel
OptionsController ..> OptionsView

PlayerController ..> MoveCommand
PlayerController ..> JumpCommand
PlayerController ..> DashCommand
PlayerController ..> AttackCommand
MoveCommand ..|> ICommand
JumpCommand ..|> ICommand
DashCommand ..|> ICommand
AttackCommand ..|> ICommand

PlayerController --> Bow
PlayerController --> TeslaGun
Bow --> ObjectPool
ObjectPool o--> Arrow

EnemySpawner --> EnemyFactory
EnemyFactory --> EnemyController

PlayerController --> KeyInventory
KeyInventory --> KeyPrototype
KeyPickup --> KeyInventory
KeyPickup --> KeyPrototype
KeyGiverNPC --> KeyInventory
KeyGiverNPC --> KeyPrototype
NPC --> KeyGiverNPC
DoorController --> KeyPrototype
DoorController --> KeyInventory

PlayerController --> PlayerStatsSubject
PlayerStatsSubject o--> IPlayerStatsObserver
HealthUI ..|> IPlayerStatsObserver
scoreUI ..|> IPlayerStatsObserver
AmmoMags --> PlayerStatsSubject
healScript --> PlayerStatsSubject
```
