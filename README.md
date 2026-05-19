# LethalLike

싱글플레이 우선 MVP(A-싱글) 기반의 PCVR(야외 행성 파밍 루프) 프로토타입 저장소입니다.

## 현재 기준

- Unity: `2022.3.46f1` (`ProjectSettings/ProjectVersion.txt`)
- XR 스택(패키지): Input System, XR Interaction Toolkit, XR Management, OpenXR
- 네트워크/코옵: **미구현** (후순위)
- 코옵 전환 대비: 판정 로직을 `IGameAuthority`/`LocalAuthority`로 분리

## 구현된 스크립트

`Assets/Scripts/Gameplay/`

- `RoundManager.cs` : Ready/Playing/Ended, 타이머, 종료 사유
- `RoundStatusUI.cs` : 남은 시간/반납 씨앗 수/점수/사유 텍스트 표시
- `TeamScoreManager.cs` : 점수/반납 카운트 관리
- `Seed.cs`, `SeedSpawner.cs` : 씨앗 데이터/스폰
- `DropZone.cs` : 반납 트리거
- `PlayerHealth.cs` : 체력/피격/사망
- `PlantEnemyAI.cs`, `EnemySpawner.cs` : 적 추적/공격/스폰
- `XRLocomotionPreset.cs` : 텔레포트 vs 스무스 이동 프리셋
- `Services/IGameAuthority.cs`, `Services/LocalAuthority.cs`
- `Services/IReviveService.cs`, `Services/SinglePlayerReviveService.cs`

## 코옵 후순위 대비 구조

다음 지점에 코옵 TODO를 명시했습니다.

- 점수 반납 판정 (`LocalAuthority.TryDeliverSeed`)
- 데미지 판정 (`LocalAuthority.TryApplyDamage`)
- 스폰 처리 (`SeedSpawner`, `EnemySpawner`)

주석 키워드:

```csharp
// TODO(coop): move this into NetworkAuthority / server-authoritative flow
```

## Test_MVP_Single 씬 구성 가이드

> 이 저장소는 블록아웃/배치를 코드로 자동 생성하지 않습니다. 아래 순서로 Unity Editor에서 씬을 1회 세팅하세요.

1. `Assets/Scenes/Test_MVP_Single.unity` 새 씬 생성
2. XRI Starter Assets 기준으로 XR Rig 배치
   - Grab/Drop 가능하도록 Interactor 설정
   - `XRLocomotionPreset`을 Rig에 추가하고 모드 1개로 고정(텔레포트 또는 스무스)
3. 매니저 오브젝트 생성
   - `TeamScoreManager`, `LocalAuthority`, `SinglePlayerReviveService`, `RoundManager` 추가
   - `RoundManager`에 `TeamScoreManager`, `PlayerHealth`, `SinglePlayerReviveService` 연결
4. 플레이어 오브젝트에 `PlayerHealth` 추가
5. Seed 프리팹 생성
   - Collider + Rigidbody + (필요 시 XR Grab Interactable)
   - `Seed` 컴포넌트 추가
   - Grab 이벤트에 `Seed.OnPickedUp`, Drop 이벤트에 `Seed.OnDropped` 연결
6. DropZone 프리팹 생성
   - Trigger Collider + `DropZone`
   - `DropZone`에 `TeamScoreManager`, `LocalAuthority` 연결
7. 적 프리팹 생성
   - `PlantEnemyAI` 추가
   - NavMeshAgent 사용 시 베이크 후 자동 추적
8. 스폰 배치
   - `SeedSpawner`, `EnemySpawner`에 프리팹/스폰포인트 할당
9. UI 텍스트(TMP) 생성
   - `RoundStatusUI`를 붙이고 `RoundManager`, `TeamScoreManager`, `TMP_Text` 연결

## 플레이 테스트 체크

- Seed를 잡아 운반하고 DropZone 진입 시 반납 카운트/점수 증가
- 적이 플레이어를 감지해 추적하고 근접 공격 시 체력 감소
- 체력 0 또는 타이머 만료 시 라운드 종료 사유 표시
- 네트워크/로비/서버 코드 추가 없음
