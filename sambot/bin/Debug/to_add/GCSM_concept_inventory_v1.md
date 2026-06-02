# GCSM Concept Inventory — v1 Draft

**Purpose**: This inventory is the foundation for the fine-tuning dataset. Every concept here will become one or more Q/A pairs that teach DeepSeek-R1 the GCSM vocabulary natively.

**Source papers consulted**:
- `GCSM-_technical_artifact_copy.docx` — formal equation, variables, guards, irreversible threshold
- `GCSM_UnifiedFrameworkPaper_v2.docx` — unified framework, all four layers, PD, CLE, Condition Branch
- `GCSM_FieldState_Paper_v1.docx` — LDI, CLAC, Field-State diagnostics
- `GCSM_CaseStudy1_ReversibilityCollapse.docx` — applied reversibility threshold crossing
- `Poverty_Compressed_Option_Space_COMPLETE_2v2.docx` — option-space compression, applied economics
- `ntelligence_as_a_Structural_Property_Under_Constraint.docx` — intelligence as structural function
- `BridgeSchool_COMPLETE_FINAL.docx` — economic Bridge function, historical case analyses
- `CONSTRAINT-GOVERNED_INQUIRY1v1.docx` — not deeply read yet, may add concepts

**Inventory structure**: each concept gets `name | symbol if applicable | layer | one-sentence definition | source paper`. Layer means which architectural layer of GCSM it belongs to.

---

## Layer 1: Foundational Mode Structure

These are the three cognition modes and the routing rules between them.

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Divergent cognition | D(t) | Variance generation under direct exposure to reality; asks "what is happening" | UnifiedFramework, Technical Artifact |
| Bridge cognition | B(t) | Switching, timing, translation, and reversibility authority between modes | UnifiedFramework, BridgeSchool |
| Convergent cognition | C(·) | Execution, stabilization, and closure; asks "what can be done" | UnifiedFramework, Technical Artifact |
| Reality ingress | — | The mechanism by which signal from real conditions enters a system | UnifiedFramework, BridgeSchool |
| Synthetic mode | — | Non-cost-bearing prosthetic reasoning (e.g., AI); operates only inside C(·) | UnifiedFramework, Technical Artifact |
| Adjacency constraint | — | Hard rule: Divergent cannot route directly to Convergent; must pass through Bridge | Technical Artifact |
| Primary routing equation | O(t) = C(G_B(t) × [D(t) × L(t) × R(t)] ÷ [1 + I(t) + P(t)]) | Formal expression of how signal becomes output | Technical Artifact |
| Bridge gate function | G_B(t) | Gating function determining when routing is permitted | Technical Artifact |
| Timing authority function | Δ(t) | Component of bridge gate; governs when, not how much | Technical Artifact |

## Layer 2: Failure Dynamics

How systems fail after they appear to succeed.

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Structural Health | — | Capacity to resolve load (not redistribute), preserve authority/cost alignment, maintain reversibility | UnifiedFramework |
| Appearance of Success | — | Surface indicators of effectiveness (output, adoption, narrative coherence) that don't assess underlying load or reversibility | UnifiedFramework |
| Load (structural) | — | Unresolved cognitive/social/systemic cost from deferred uncertainty, suppressed signal, translation work | UnifiedFramework |
| Authority Drift | — | Process by which decision authority migrates away from cost-bearing exposure | UnifiedFramework |
| Narrative overcompression | — | Complexity reduced faster than it is resolved (early warning signal) | UnifiedFramework |
| Acceleration substitution | — | Speed replacing evaluation as the signal of competence (early warning) | UnifiedFramework |
| Synthetic coherence | — | AI/synthetic output filling signal gaps, confirming assumptions without new signal | UnifiedFramework |
| Audience interpretation drift | — | Growing gap between producer outputs and receiver experience | UnifiedFramework |
| Bridge signal suppression | — | Warnings from cost-bearing layers filtered before reaching authority | UnifiedFramework |
| Functional reward continuity | — | Success metrics staying stable as structural alignment deteriorates | UnifiedFramework |
| Psychofunctional Delusion | PD | Convergent failure mode where system continues functionally rewarded output while reality ingress is severed | UnifiedFramework, BridgeSchool |
| PD four structural conditions | — | (1) exposure misalignment, (2) functional reward continuity, (3) constraint reinforcement preventing correction, (4) branch stability sufficient to sustain output | UnifiedFramework |
| Bridge Impersonation | — | Institutional roles claiming translation authority without cost-bearing exposure | BridgeSchool |

## Layer 3: Constraint and State Expression (CLE + Condition Branch)

The substrate of energetic/processing constraints, and how states emerge from constraint interaction.

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Cognitive Load and Energy Layer | CLE | Non-identity, non-governance substrate defining energetic/processing constraints | UnifiedFramework, FieldState |
| Load | L | Total demand on the system including baseline and intervention-induced | UnifiedFramework |
| Capacity | K | Maximum sustainable load while maintaining functional integrity; not static | UnifiedFramework |
| Latent Obligation Drag | LOD | Load from unresolved/deferred/implicitly held obligations not actively executed but consuming capacity | UnifiedFramework, BridgeSchool |
| Effective Capacity | K_eff | K − LOD; the operative boundary for all viability assessments | UnifiedFramework |
| Saturation Threshold | S | Relational state at which additional load causes nonlinear degradation | UnifiedFramework |
| Recovery Rate | R | Rate of capacity restoration after strain | UnifiedFramework |
| Switching Cost | SC | Energy required to transition between states; SC ≤ (K_eff − L) for permitted transitions | UnifiedFramework |
| Noise Sensitivity | N | Vulnerability to interference | UnifiedFramework |
| Background Load | BL | Ongoing non-visible processing | UnifiedFramework |
| Condition Branch | f(L,E,T,C) | Localized temporary structural state from interaction of load, exposure, timing, constraint | UnifiedFramework |
| Stable Functional (CB pattern) | — | Load within K_eff; exposure aligned; consistent output | UnifiedFramework |
| Strained Functional (CB pattern) | — | Load approaching K_eff; functional but effortful | UnifiedFramework |
| Degraded (CB pattern) | — | Load exceeds K_eff; exposure misaligned; reduced accuracy | UnifiedFramework |
| Distorted (CB pattern) | — | Exposure significantly misaligned; internally coherent but externally invalid | UnifiedFramework |
| Locked (CB pattern) | — | Transition pathways eliminated; reversibility removed | UnifiedFramework |
| Collapsing (CB pattern) | — | Thresholds exceeded beyond recovery | UnifiedFramework |
| Multiplex (CB pattern) | — | Different subsystems under different constraint profiles | UnifiedFramework |
| Load Pruning | — | Increasing L reduces available behavioral pathways | UnifiedFramework |
| Saturation Collapse | — | When L ≥ K_eff, Bridge function cannot permit transitions | UnifiedFramework |
| Transition Filtering | — | SC ≤ (K_eff − L) required for permitted transitions | UnifiedFramework |
| LOD-Induced Distortion | — | LOD reduces K_eff without visible load increase | UnifiedFramework |
| Stabilization Pressure | — | Increasing constraint locks the active branch progressively | UnifiedFramework |
| False Closure | — | LOD-induced capacity reduction causing premature branch stabilization; constraint mechanism underlying PD | UnifiedFramework |
| Load Distribution Index | LDI | Topology of how load is distributed across active processing subsystems | FieldState |
| Cyclic Load Attractor Condition | CLAC | Degradation dynamic where system cycles between near-saturation and partial recovery without progressing along reversibility thresholds | FieldState |
| Field-State | — | Composite condition where both LDI distribution and CLAC cycling are active | FieldState |

## Layer 4: Reversibility / Threshold Systems

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Reversibility | — | Ability to return to a prior functional state without new structural cost or losing future transition capacity | UnifiedFramework |
| Reversibility Window | R_v | Range of system states within which transitions/rollback can occur without structural penalty | UnifiedFramework |
| Reversibility Coefficient | R(t) | Value 0-1 representing availability of safe rollback, pause, or exit | Technical Artifact |
| Irreversible Threshold | — | When R(t) = 0, lawful convergence is no longer possible (structural limit) | Technical Artifact |
| RT-1: Constraint Saturation | — | Accumulated constraint exceeds processing capacity; reversibility window narrows | UnifiedFramework |
| RT-2: Transition Cost Exceedance | — | Cost of changing state exceeds available capacity | UnifiedFramework, CaseStudy |
| RT-3: Temporal Closure | — | Decision window closes before correction can occur | CaseStudy |
| RT-4: Structural Lock-in | — | System configuration prevents exit without introducing new structural cost | UnifiedFramework, CaseStudy |

## Cross-Cutting / Structural Properties

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Cost-Bearing Locality | L(t) | 0-1 indicating how much cost is borne at the signal source | Technical Artifact |
| Insulation Factor | I(t) | Degree to which signal origin is shielded from downstream consequences (penalty term) | Technical Artifact |
| Power Asymmetry Factor | P(t) | Imbalance between decision authority and cost exposure | Technical Artifact |
| Reversibility Guard | — | If R(t) ≤ 0 then G_B(t) = 0 and O(t) = 0 | Technical Artifact |
| Cost vs Power Guard | — | If L(t) ≤ P(t) then G_B(t) = 0 | Technical Artifact |
| Timing Guard | — | If Δ(t) = 0 then G_B(t) = 0 (acceleration cannot substitute for readiness) | Technical Artifact |
| Extraction Penalty | — | As I(t) or P(t) increases, output decreases continuously | Technical Artifact |

## Intelligence-as-Structural-Property (separate paper, parallel framework)

| Name | Definition | Source |
|---|---|---|
| Intelligence (structural definition) | Capacity of a system to maintain reality contact, route signal without distortion, preserve reversibility, align authority with cost, sustain function under constraint | Intelligence Paper |
| Signal Routing Integrity | Ability to transmit information across cognitive processes without distortion, suppression, or premature stabilization | Intelligence Paper |
| Constraint Navigation Capacity | Ability to maintain function as constraint conditions increase | Intelligence Paper |
| Failure Without Incompetence | Central implication that failure does not require individual incompetence — structural conditions degrade intelligence | Intelligence Paper |

## Bridge School / Economic Application

| Name | Definition | Source |
|---|---|---|
| Bridge School of Economic Thought | Application of GCSM to economic systems; reframes economics through timing, cost, structural alignment | BridgeSchool |
| Divergent system (economic) | Decentralized signal-generating system (Austrian mapping) | BridgeSchool |
| Convergent system (economic) | Coordinated/centralized stabilization system (Keynesian mapping) | BridgeSchool |
| Bridge function (economic) | Function governing transition between signal generation and coordinated action | BridgeSchool |
| Genuine Bridge Governance | Cost-bearing exposure + structural independence from Convergent + reality contact + reversibility authority + downward accountability | BridgeSchool |
| Economic misalignment | When signal generation, decision authority, and cost-bearing lose coordination | BridgeSchool |

## Applied: Poverty as Compressed Option Space

| Name | Symbol | Definition | Source |
|---|---|---|---|
| Compressed Option Space | — | Structural condition where viable paths narrow under sustained constraint interaction | Poverty Paper |
| Human Trust State | H | Variable representing trust-state required to access available options | Poverty Paper |
| System Signal Quality | M | Variable representing whether signals about available options are accurate and accessible | Poverty Paper |

---

## What's NOT yet in this inventory (flagging gaps)

1. **CONSTRAINT-GOVERNED_INQUIRY1v1.docx** — I haven't read this carefully yet. Likely adds admissibility-layer vocabulary I'm missing.
2. **The five irreducible claims** of GCSM — UnifiedFramework references "the model's irreducible minimal form in five statements" but I didn't pull the five statements specifically.
3. **The five formal constraint invariants** — same, mentioned but not pulled.
4. **Cross-scale walkthrough specifics** — Section 6C of UnifiedFramework discusses individual / organizational / governance / distributed scales applying the same mechanics; specific examples would matter for fine-tuning.
5. **Scope guards** appear throughout but I haven't catalogued them — they're important because they tell the model what GCSM *doesn't* claim, and a fine-tuned model should know to refuse those claims.

---

## Questions for you before drafting Q/A pairs

1. **Is this list complete enough to start drafting?** If you say yes, I'll fill in the gaps above incrementally as we go. If you say no, point me at concepts I missed and I'll search the corpus for them.

2. **Anything misnamed or mis-categorized?** I made judgment calls on which layer some concepts belong to. Tell me where I got it wrong.

3. **Priority for drafting Q/A pairs:** which layer or concept group should we start with? Options:
   - Start with **definitions** of every core variable (L, K, LOD, K_eff, etc.) — pure recall
   - Start with **relationships** (how does LOD relate to K_eff; why must Divergent route through Bridge) — reasoning
   - Start with **failure modes** (PD, authority drift, false closure) — applied
   - Start with **scope guards** (what GCSM does NOT claim) — refusal/limit awareness

4. **Tone of model responses:** what do you want the fine-tuned model to *sound* like when explaining GCSM? Academic and formal (like your papers)? Conversational? Should it use "we" / "the framework" / your name? This affects every training pair.
