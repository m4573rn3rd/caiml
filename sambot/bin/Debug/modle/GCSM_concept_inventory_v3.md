# GCSM Concept Inventory — v3 Draft

**Purpose**: Foundation for the fine-tuning dataset. Every concept becomes Q/A pair(s).

**v3 changes from v2**: Added WP2 (Failure-in-Time) diagnostic vocabulary and WP4 (Reversibility & Threshold Systems) formal definition block. ~30 new concepts.

**Source papers consulted** (now 11):
- `GCSM-_technical_artifact_copy.docx` — formal equation, variables, guards
- `GCSM_UnifiedFrameworkPaper_v2.docx` — unified framework, 5 irreducible statements, 5 invariants
- `WhitePaper2Failure-in-Time.docx` — **NEW**: diagnostic layer, early warning signals, bridge erosion patterns, irreversibility gates, reversibility tax
- `GCSM_WP_4v1_Reversibility_Threshold.docx` — **NEW**: formal reversibility-layer definitions, threshold types
- `GCSM_WP5_FailureSuccessModes.docx` — 9 paired failure/success modes, 6 meta-failures, 4 underlying conditions
- `GCSM_FieldState_Paper_v1.docx` — LDI, CLAC, Field-State
- `GCSM_CaseStudy1_ReversibilityCollapse.docx` — applied case
- `Poverty_Compressed_Option_Space_COMPLETE_2v2.docx` — applied economics
- `ntelligence_as_a_Structural_Property_Under_Constraint.docx` + `TheStructureOfIntelligence_Clean.docx` — intelligence paper + book form
- `BridgeSchool_COMPLETE_FINAL.docx` — economic Bridge function
- `CONSTRAINT-GOVERNED_INQUIRY1v1.docx` — admissibility layer
- `Education_Failure_Without_Incompetence_v3.docx` — applied to education

---

## Layer 1: Foundational Mode Structure

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Divergent cognition | D(t) | Variance generation under direct exposure to reality; asks "what is happening" | UnifiedFramework, Technical Artifact |
| Bridge cognition | B(t) | Switching, timing, translation, and reversibility authority between modes — defined by observable function, not identity/trait/status | UnifiedFramework, BridgeSchool, WP2 |
| Convergent cognition | C(·) | Execution, stabilization, and closure; asks "what can be done" | UnifiedFramework, Technical Artifact |
| Reality ingress | — | Mechanism by which signal from real conditions enters a system | UnifiedFramework, BridgeSchool |
| Synthetic mode | — | Non-cost-bearing prosthetic reasoning (e.g., AI); operates only inside C(·); not a fourth cognition mode | UnifiedFramework, Technical Artifact, WP5 |
| Adjacency constraint | — | Hard rule: Divergent cannot route directly to Convergent; must pass through Bridge | Technical Artifact, WP5 |
| Primary routing equation | O(t) = C(G_B(t) × [D(t) × L(t) × R(t)] ÷ [1 + I(t) + P(t)]) | Formal expression of how signal becomes output | Technical Artifact |
| Bridge gate function | G_B(t) | Gating function determining when routing is permitted | Technical Artifact |
| Timing authority function | Δ(t) | Component of bridge gate; governs when, not how much | Technical Artifact |
| Bridge legitimacy condition | — | A bridge is legitimate only if it can alter outcomes, not merely interpret them | WP2 |

## Layer 2: Failure Dynamics

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Structural Health | — | Capacity to resolve load (not redistribute), preserve authority/cost alignment, maintain reversibility | UnifiedFramework, WP2 |
| Appearance of Success | — | Surface indicators (output, adoption, narrative coherence, formal legitimacy, reduced visible conflict) that don't assess underlying load or reversibility | UnifiedFramework, WP2 |
| Load (structural) | — | Unresolved cognitive/social/systemic cost from deferred uncertainty, suppressed signal, translation work; NOT synonymous with effort/stress/workload | UnifiedFramework, WP2 |
| Authority Drift | — | Process by which decision authority migrates away from cost-bearing exposure toward insulated layers over time | UnifiedFramework, WP2 |
| Narrative overcompression | — | Complex unresolved realities prematurely collapsed into simplified explanatory stories; reduces narrative load while increasing structural load (early warning) | UnifiedFramework, WP2 |
| Acceleration substitution | — | Increased speed used as proxy for understanding/resolution; functions as avoidance, not improvement (early warning) | UnifiedFramework, WP2 |
| Synthetic coherence (failure mode) | — | Internal consistency generated without ongoing contact with reality ingress; "confidence without exposure" | UnifiedFramework, WP2, WP5 |
| Audience interpretation drift | — | Meaning of system outputs diverges between those closest to consequence and those furthest from it | UnifiedFramework, WP2 |
| Bridge signal suppression | — | Warnings from cost-bearing layers filtered before reaching authority | UnifiedFramework |
| Functional reward continuity | — | Success metrics staying stable as structural alignment deteriorates | UnifiedFramework |
| Psychofunctional Delusion | PD | Convergent failure: system continues functionally rewarded output while reality ingress is severed | UnifiedFramework, BridgeSchool, WP5 |
| PD four structural conditions | — | (1) exposure misalignment, (2) functional reward continuity, (3) constraint reinforcement preventing correction, (4) branch stability sufficient to sustain output | UnifiedFramework |
| Bridge Impersonation | — | Translation authority claimed without cost-bearing exposure, lived contradiction, or switching authority | BridgeSchool, WP2, WP5 |
| Failure Without Incompetence | — | Central claim: system can fail despite adequate capability in all participants; failure attributed to structure, not agents | Intelligence Paper, Education Paper, Structure of Intelligence |
| Misattribution Error | — | Tendency to interpret structural outcomes as individual deficiency; self-perpetuating because it directs reform at people instead of architecture | Education Paper |

## Layer 2 — NEW: WP2 Diagnostic Vocabulary (early warning signals and erosion patterns)

| Name | Definition | Source |
|---|---|---|
| **Convergent Dependency Anxiety** | Stabilized systems become dependent on continued input from divergent/bridge processes they no longer tolerate; appears as pressure for premature closure | WP2 §3.5 |
| **Bridge Signal Emergence** | Translation attempts that indicate structural strain — repeated efforts to contextualize, warnings framed as timing issues, attempts to slow/pause/reframe | WP2 §3.6 |
| **Bridge Erosion** | Bridge integrity degrades under sustained unresolved load: translation labor increases without authority increase, pressure to compress nuance, framing delay as obstruction | WP2 §4.3 |
| **Bridge Capture** | Bridge bypassed, overruled, exhausted, or captured into procedural roles | WP2 §4.3 |
| **Governance Delay** | Delay that preserves reversibility — produces correction capacity | WP2 §4.4 |
| **Weaponized Delay** | Delay used to exhaust dissent, defer correction, entrench commitments — distinct from governance delay (WP2's name for what WP5 formalizes as Delay-as-Weapon) | WP2 §4.4 |
| **Reversibility Tax** | Increasing cost imposed on those who attempt to pause/exit/redirect a system; collapse occurs when cost of leaving exceeds cost of staying | WP2 §5.4 |
| **Irreversibility Gate** | A decision or event converting reversible momentum into fixed trajectory (large-scale deployment, public commitment, legal codification, institutionalization, automation transfer); frequently treated as success milestone | WP2 §5.5 |
| **Synthetic Pressure** | Pressure to delegate interpretation/coordination/judgment to synthetic systems as systems scale; framed as efficiency/neutrality/risk reduction | WP2 §6 |
| **Authority Displacement** | Distinct failure pattern from synthetic pressure; precursor to ADF (formalized in WP5) | WP2 §6 |
| **Late-Stage Intervention Cost** | Cost of correction increases nonlinearly as reversibility narrows; late-stage intervention consistently requires more structural disruption than early-stage | UnifiedFramework, WP4 |

## Layer 3: Constraint and State Expression (CLE + Condition Branch)

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Cognitive Load and Energy Layer | CLE | Non-identity, non-governance substrate defining energetic/processing constraints | UnifiedFramework, FieldState |
| Load | L | Total demand including baseline and intervention-induced | UnifiedFramework, CGI |
| Capacity | K | Maximum sustainable load; not static — can decrease under stress, be temporarily reduced by interventions, or impaired by saturation/damage | UnifiedFramework, CGI |
| Latent Obligation Drag | LOD | Load from unresolved/deferred/implicit obligations consuming capacity before becoming active; forward-binding capacity commitment | UnifiedFramework, BridgeSchool, CGI |
| Effective Capacity | K_eff | K − LOD; operative boundary for all viability assessments | UnifiedFramework, CGI |
| Saturation Threshold | S | Relational state at which additional load causes nonlinear degradation; amplifier, not subtractor | UnifiedFramework, CGI |
| Recovery Rate | R | System's ability to restore capacity after load reduction; degraded by sustained load and repeated intervention | UnifiedFramework, CGI, WP4 |
| Switching Cost | SC | Energy required to transition between states; SC ≤ (K_eff − L) for permitted transitions | UnifiedFramework |
| Noise Sensitivity | N | Vulnerability to interference | UnifiedFramework |
| Background Load | BL | Ongoing non-visible processing | UnifiedFramework |
| Condition Branch | f(L,E,T,C) | Localized temporary structural state from interaction of load, exposure, timing, constraint | UnifiedFramework |
| Stable Functional (CB) | — | Load within K_eff; exposure aligned; consistent output | UnifiedFramework |
| Strained Functional (CB) | — | Load approaching K_eff; functional but effortful | UnifiedFramework |
| Degraded (CB) | — | Load exceeds K_eff; exposure misaligned; reduced accuracy | UnifiedFramework |
| Distorted (CB) | — | Exposure significantly misaligned; internally coherent but externally invalid | UnifiedFramework |
| Locked (CB) | — | Transition pathways eliminated; reversibility removed | UnifiedFramework |
| Collapsing (CB) | — | Thresholds exceeded beyond recovery | UnifiedFramework |
| Multiplex (CB) | — | Different subsystems under different constraint profiles | UnifiedFramework |
| Load Pruning | — | Increasing L reduces available behavioral pathways | UnifiedFramework |
| Saturation Collapse | — | When L ≥ K_eff, Bridge function cannot permit transitions | UnifiedFramework |
| Transition Filtering | — | SC ≤ (K_eff − L) required for permitted transitions | UnifiedFramework |
| LOD-Induced Distortion | — | LOD reduces K_eff without visible load increase | UnifiedFramework |
| Stabilization Pressure | — | Increasing constraint locks the active branch progressively | UnifiedFramework |
| False Closure | — | LOD-induced capacity reduction causing premature branch stabilization; constraint mechanism underlying PD | UnifiedFramework |
| Load Distribution Index | LDI | Topology of how load is distributed across active processing subsystems | FieldState |
| Cyclic Load Attractor Condition | CLAC | System cycles between near-saturation and partial recovery without progressing along reversibility thresholds | FieldState |
| Field-State | — | Composite condition where both LDI distribution and CLAC cycling are active | FieldState |

## Layer 4: Reversibility / Threshold Systems (EXPANDED from WP4)

### WP4 Formal Definition Block

| Name | Definition | Source |
|---|---|---|
| **Reversibility** | Ability of a system to return to a prior functional state without incurring new structural cost or losing future transition capacity | WP4 §3, UnifiedFramework |
| **Irreversibility** | Condition in which returning to a prior functional state requires additional structural cost, introduces new constraints, or is no longer possible | WP4 §3 |
| **Reversibility Window** (R_v) | Range of system states within which transitions, rollback, or redirection can occur without structural penalty | WP4 §3, UnifiedFramework, CGI |
| **Threshold** | Boundary condition at which a system undergoes a state change that alters reversibility | WP4 §3 |
| **Recovery** | Process by which a system reduces accumulated constraint and restores transition capacity | WP4 §3 |
| **Collapse** | State in which system function persists but transition capacity is no longer available | WP4 §3 |
| **Lock-in** | Condition where system remains operational but cannot exit current state without external intervention or structural reset | WP4 §3 |
| **Reversibility Coefficient** | R(t) value 0-1 representing availability of safe rollback, pause, or exit | Technical Artifact |
| **Irreversible Threshold** | When R(t) = 0, lawful convergence is no longer possible (structural limit, not policy) | Technical Artifact |

### The Four Threshold Classes (WP4 §4)

| Class | Name | Trigger | Structural Effect | Reversibility Status |
|---|---|---|---|---|
| RT-1 | Constraint Saturation Threshold | Load accumulation persists while recovery insufficient | Reversibility window narrows | Narrowing; recovery possible but increasingly constrained |
| RT-2 | Transition Cost Threshold | SC rises above available capacity for transition execution; SC > (K_eff − L) | State transitions become delayed, avoided, or replaced with stabilization | Conditional; recovery requires SC reduction before movement |
| RT-3 | Temporal Closure Threshold | Feedback delay exceeds time available for system adjustment | System stabilizes prematurely, preventing correction/rollback | Reduced; reversal requires reopening closed timing conditions |
| RT-4 | Structural Lock-in Threshold | Exit conditions require additional constraint, capacity loss, or external intervention | System remains in current state regardless of internal variation | Collapsed; recovery not possible within current system conditions |

**Canonical crossing order**: RT-1 → RT-2 → RT-3 → RT-4

## Cross-Cutting / Structural Variables

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Cost-Bearing Locality | L(t) | 0-1 indicating how much cost is borne at the signal source | Technical Artifact |
| Insulation Factor | I(t) | Degree to which signal origin is shielded from downstream consequences (penalty term) | Technical Artifact |
| Power Asymmetry Factor | P(t) | Imbalance between decision authority and cost exposure | Technical Artifact |
| Reversibility Guard | — | If R(t) ≤ 0 then G_B(t) = 0 and O(t) = 0 | Technical Artifact |
| Cost vs Power Guard | — | If L(t) ≤ P(t) then G_B(t) = 0 | Technical Artifact |
| Timing Guard | — | If Δ(t) = 0 then G_B(t) = 0 (acceleration cannot substitute for readiness) | Technical Artifact |
| Extraction Penalty | — | As I(t) or P(t) increases, output decreases continuously | Technical Artifact |

---

## The Five Irreducible Statements

| # | Statement | Source |
|---|---|---|
| 1 | Reality enters systems through exposure, not abstraction | UnifiedFramework §6A |
| 2 | Translation between reality contact and stable action requires a governing mechanism (Bridge) | UnifiedFramework §6A |
| 3 | Stabilization downstream of translation is necessary but not self-authorizing (Convergent has execution authority, not signal authority) | UnifiedFramework §6A |
| 4 | Effective capacity is always capacity minus forward-binding obligations (K_eff = K − LOD) | UnifiedFramework §6A |
| 5 | Correction is only possible while reversibility is preserved | UnifiedFramework §6A |

## The Five Formal Constraint Invariants

| # | Invariant | Condition | Source |
|---|---|---|---|
| 1 | Effective capacity is always K minus LOD | K_eff = K − LOD; LOD ≥ 0; K_eff ≤ K always | UnifiedFramework §6B |
| 2 | Transitions structurally permitted only when SC ≤ available capacity margin | SC ≤ (K_eff − L) ⇔ transition permitted | UnifiedFramework §6B |
| 3 | Saturation onset is a nonlinear amplifier, not a linear threshold | As L → K_eff, marginal impact of additional L increases nonlinearly | UnifiedFramework §6B |
| 4 | Reversibility loss is cumulative and non-reversing without external structural input | Once RT-n crossed, return requires cost > (K_eff − L) | UnifiedFramework §6B |
| 5 | Bridge function is a necessary condition for valid system transition | Valid transition ⇒ Bridge function active and cost-bearing | UnifiedFramework §6B |

---

## WP5 — Paired Failure/Success Modes

### Pairing Criteria (required for valid pairing)
1. **Shared mechanism** — same underlying interaction of load/capacity/SC/LOD/Bridge
2. **Layer correspondence** — same structural layer (no cross-layer pairings)
3. **Reversibility relationship** — transition from failure to success structurally specified
4. **No moral inversion** — both states are structural; not bad-vs-good

### Divergent Layer

| Failure Mode | Success Mode | Shared Mechanism |
|---|---|---|
| **Dissolution** — variance generation exceeds stabilization capacity; continuous emergence without containment | **Emergence Success** — reality ingress fully forms before premature stabilization; variance kept survivable; recovery preserved | Ratio between variance generation and stabilization timing |
| **Premature Convergence** — convergence closes uncertainty before sufficient divergence completed | **Conditional Stabilization** — convergence executes without claiming permanent epistemic authority; treated as operational but provisional | Same convergence operation; framing of authority is the structural variable |

### Bridge Layer

| Failure Mode | Success Mode | Shared Mechanism |
|---|---|---|
| **Bridge Impersonation** — bridge translation without cost-bearing exposure, lived contradiction, or switching authority | **Legitimate Transition Authority** — bridge governs timing/translation/reversibility/exposure, exercised by authority that bears cost | Same translation mechanics; locus of accountability is the structural variable |
| **Delay-as-Weapon** — delay used to exhaust challengers, defer correction, entrench convergent commitments | **Exposure Safety** — timing protects signal sources rather than depleting them; truth expression becomes safe enough for sustainable visibility | Whether delay distributes load symmetrically or asymmetrically |
| **Bridge Collapse** — bridge loses authority/capacity/legitimacy; no mediating function between divergent and convergent | **Distributed Glow** — bridge function distributed across multiple cost-bearing actors; produces shared confidence and reduced load | Whether bridge legitimacy is held by structural condition or individual position |

### Convergent Layer

| Failure Mode | Success Mode | Shared Mechanism |
|---|---|---|
| **Psychofunctional Delusion** — functional outputs continue while reality ingress severed | **Load Resolution Success** — output rises/stabilizes while load decreases; rising output with falling load is resolution; rising output with rising load is extraction | Whether output is decoupled from or aligned with reality ingress |
| **Synthetic Coherence** — artificial stability through narrative/metrics/synthetic systems/abstraction without reality ingress | **Reversibility Preservation** — real (not symbolic) capacity for adaptation, correction, withdrawal, transition | Whether stability is achieved by suppressing variation or absorbing it |
| **Lock-In Engineering** — structural choices progressively reducing reversibility | **Reversibility Custodianship (Governance)** — governance actively protects exit paths and correction capacity as primary structural responsibility | Whether reversibility is treated as default or as actively maintained |

### Governance Layer

| Failure Mode | Success Mode | Shared Mechanism |
|---|---|---|
| **Authority Displacement Failure (ADF)** — authority delegated to systems that don't bear consequences | **Authority Decay Recognition** — systems recognize when authority validity has expired; permit dignified transition | Whether authority limits are accommodated through displacement or succession |

### The Reduced Structural Spine (4 conditions underlying all 9 paired modes)

Per WP5 §5.1, the 9 modes reduce to broken/intact routing between four conditions:

1. **Reality ingress** (Dissolution ↔ Emergence Success)
2. **Translation authority** (Bridge Impersonation ↔ Legitimate Transition Authority)
3. **Stabilization pressure** (Premature Convergence ↔ Conditional Stabilization)
4. **Reversibility** (Lock-In Engineering ↔ Reversibility Custodianship)

### WP5 Meta-Principle

*Most large-scale failures are not caused by evil actors or insufficient intelligence, but by broken routing between reality ingress, translation authority, stabilization pressure, and reversibility. Most large-scale successes are not caused by extraordinary actors or unusual conditions, but by intact routing across the same four structural variables.*

---

## WP5 — Meta-Failure Modes of GCSM (paired with hygiene principles)

| # | Meta-Failure | Hygiene Principle |
|---|---|---|
| 4.1 | **Taxonomy Bloat** — proliferation of subtypes beyond what structural mechanics require | **Few modes, many expressions**: framework holds small number of structurally distinct modes; new labels added only when identifying mechanisms not already covered |
| 4.2 | **Mythologization** — framework becomes mystical, identity-based, hero-oriented | **Structure, not mythology**: framework describes structural conditions, not identities/virtues/destinies; no mode is morally privileged |
| 4.3 | **Identity Capture** — cognitive modes interpreted as personality types or moral categories | **Modes, not traits**: cognition is mode-based engagement; individuals/systems move between modes over time; no person *is* a mode |
| 4.4 | **Synthetic Expansion Failure** — synthetic reasoning misclassified as a fourth cognition mode rather than convergent prosthetic | **Three modes only; synthetic is prosthetic**: only D/B/C are cognition modes; synthetic systems are convergent prosthetics — don't bear cost, don't generate reality ingress, don't exercise bridge authority |
| 4.5 | **Direct Adjacency Failure** — D and C treated as directly connected, bypassing Bridge | **All legitimate transitions pass through bridge**: D and C are not directly adjacent; framework application omitting bridge governance is structurally invalid |
| 4.6 | **Compression Violence** — complex emergence collapsed into simplified success narratives that remove visible cost of stabilization | **Preserve temporal cost in description**: applications retain temporal cost of emergence/stabilization/transition |

---

## WP2 Diagnostic Misuse Guardrails (NEW SECTION)

These are critical for the fine-tune: the model should refuse misuse of diagnostic tools.

| Misuse Pattern | Description | Source |
|---|---|---|
| **Diagnostic Automation** | Diagnostics applied automatically, detached from cost-bearing context, used to replace human judgment | WP2 §8.1 |
| **Identity Application** | Using diagnostics to classify people/roles/groups; rank individuals; assess merit/capability; justify exclusion/discipline | WP2 §8.2 |
| **Retrospective Justification** | Applying diagnostics after outcomes are fixed to legitimize decisions already made | WP2 §8.3 |
| **Selective Application** | Using diagnostics to scrutinize dissent while exempting authority | WP2 §8.3 |
| **Surveillance Use** | Diagnostics applied to monitor behavior at scale | WP2 §8.2 |

### Non-Automation Constraint (WP2)

> No diagnostic is valid if applied automatically, detached from cost-bearing context, used to replace human judgment, or embedded in synthetic systems as decision authority. Synthetic systems may assist in pattern visibility but may not determine diagnostic legitimacy, timing, or closure.

---

## Constraint-Governed Inquiry (CGI) — Admissibility Layer

### Minimal Closed Variable Set
**{L, K, LOD, K_eff, S, R, R_v}** — no additional variables required

### Non-Compensatory Constraint Logic
Satisfaction of one constraint cannot offset violation of another. All constraints hold simultaneously. No trade-offs at admissibility level.

### The Five CGI Constraint Conditions (strict dependency order)

| # | Condition | Formal Expression | Failure Mode |
|---|---|---|---|
| 1 | Reversibility Preservation | System state ∈ R_v throughout intervention | Lock-in or collapse |
| 2 | Load–Capacity Boundary | L_baseline + L_intervention ≤ K_eff = K − LOD | Overload-induced failure |
| 3 | Saturation Boundary | S_post < S_critical | Non-linear instability |
| 4 | Recovery Preservation | ΔR ≥ 0 (or bounded negative within reversible window) | Cumulative degradation |
| 5 | Latent Obligation Constraint | LOD_post ≤ LOD_absorbable = f(K, R, R_v) | Future capacity collapse |

### Admissibility Function
A(I) = ⋀ᵢ cᵢ(I) = 1 for all c_i ∈ C. Short-circuit: failure at any stage terminates evaluation.

---

## Intelligence as Structural Property

| Name | Definition | Source |
|---|---|---|
| Intelligence (structural definition) | Capacity to maintain reality contact, route signal without distortion, preserve reversibility, align authority with cost, sustain function under constraint | Intelligence Paper, Structure of Intelligence |
| Trait Model | Treats intelligence as fixed internal characteristic | Structure of Intelligence Ch. 1 |
| Structural Property | Characteristic emerging from how system is organized and how it interacts with conditions | Structure of Intelligence Ch. 1 |
| Signal Routing Integrity | Ability to transmit information without distortion, suppression, or premature stabilization | Intelligence Paper |
| Constraint Navigation Capacity | Ability to maintain function as constraint conditions increase | Intelligence Paper |
| Structural Failure | Failure when system produces incorrect/degraded outcomes due to operating constraints, not lack of capability | Structure of Intelligence Ch. 2 |
| Constraint Pressure | Total demand relative to capacity (time limits, info overload, environmental instability, competing demands) | Structure of Intelligence Ch. 2 |

### Five Structural Components of Intelligence
1. Signal integrity
2. Routing legitimacy
3. Reversibility capacity
4. Cost-bearing alignment
5. Constraint-condition performance

---

## Bridge School / Economic Application

| Name | Definition | Source |
|---|---|---|
| Bridge School of Economic Thought | Application of GCSM to economic systems; reframes through timing, cost, structural alignment | BridgeSchool |
| Divergent system (economic) | Decentralized signal-generating system (Austrian mapping) | BridgeSchool |
| Convergent system (economic) | Coordinated/centralized stabilization system (Keynesian mapping) | BridgeSchool |
| Bridge function (economic) | Function governing transition between signal generation and coordinated action | BridgeSchool |
| Genuine Bridge Governance | Cost-bearing exposure + structural independence from Convergent + reality contact + reversibility authority + downward accountability | BridgeSchool |
| Economic misalignment | When signal generation, decision authority, and cost-bearing lose coordination | BridgeSchool |

## Education Application

| Name | Definition | Source |
|---|---|---|
| Convergent-first architecture | Education system organized so evaluation/selection occur before sufficient divergence and bridging | Education Paper |
| Routing process between modes | Effective learning depends on D→B→C sequencing, not content delivery alone | Education Paper |
| Reversibility Collapse in Education | Grading systems assign outcomes early, accumulate, are difficult to revise — produce risk-avoidance | Education Paper |
| Self-perpetuating misattribution | System produces failure → explains as individual deficiency → eliminates own incentive to change | Education Paper |
| Role-constraint mismatch | When bridge authority is structurally constrained, produces chronic stress/burnout | Education Paper |

## Applied: Poverty as Compressed Option Space

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Compressed Option Space | — | Structural condition where viable paths narrow under sustained constraint interaction | Poverty Paper |
| Human Trust State | H | Variable representing trust-state required to access available options | Poverty Paper |
| System Signal Quality | M | Variable representing whether signals about options are accurate and accessible | Poverty Paper |
| Effective Planning Horizon | T_h | Contracts under sustained constraint, removing delayed-payoff options from perceived choice set before capacity constraints tested | Poverty Paper 2v2 |
| Social Capital Supplement | SC_s | Additional capacity variable | Poverty Paper 2v2 |
| Institutional Memory Load | I_m | Additional capacity variable | Poverty Paper 2v2 |
| Compression Trap | — | Material failure mode: L > K_eff blocks action | Poverty Paper 2v2 |
| Perception Trap | — | Temporal/signal failure mode: T_h compression and low trust remove options from awareness | Poverty Paper 2v2 |

---

## Scope Guards — What GCSM Does NOT Claim

These are critical training targets — model should refuse claims framework disclaims:

- GCSM does NOT provide diagnostic classifications of individuals or groups
- GCSM does NOT prescribe interventions, policies, or remedies
- GCSM does NOT make moral, ethical, or political claims
- GCSM does NOT replace empirical investigation
- GCSM does NOT provide psychometric or behavioral measurements
- GCSM does NOT make clinical, therapeutic, or medical claims
- Cognitive modes are NOT identities, personality types, or permanent classifications
- No individual *has* a mode; no person *is* a mode
- Synthetic systems do NOT have signal authority or reversibility authority
- Synthetic systems may NOT be classified as a fourth cognition mode
- The framework is NOT a worldview, mystical doctrine, or hero narrative
- Failure modes are NOT moral failings; success modes are NOT virtues
- Pairing failure→success does NOT imply the success mode is the goal
- Diagnostics may NOT be automated, used for surveillance, applied to individuals
- WP2 diagnostics are invalid if used as enforcement mechanisms
- GCSM is NOT empirical research; it is a structural framework

---

## Inventory Stats

- **~180 concepts** catalogued across ~12 sections
- 11 source papers consulted
- All structural layers covered: foundational modes, failure dynamics, diagnostics (WP2), CLE constraints, condition branches, reversibility/thresholds (WP4), CGI admissibility, paired failure/success modes (WP5), meta-failures, scope guards
- Applied vocabulary catalogued: economics (BridgeSchool, Poverty), education, intelligence
- Misuse-prevention vocabulary catalogued: scope guards + WP2 misuse patterns

## Open Questions Before Drafting Q/A Pairs

1. **Accuracy check** — anything misnamed or miscategorized?
2. **Anything still missing?** — particularly: are there concepts in WP3 (Constraint-Routing Interaction) that I'm getting only via downstream papers' references but should source directly? WP3 wasn't in the project files.
3. **Drafting priority** — which section first?
4. **Tone** — academic-formal matching your papers; should there also be a "conversational explainer" mode?
5. **Voice** — "the framework" / "GCSM" / "Sanchez (2026)" / first-person "I"?

Your move.
