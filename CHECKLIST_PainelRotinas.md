# Checklist - Painel Rotinas

Notas para retomar o trabalho no PainelRotinas.

## A. Slots de rotina

- [x] Corrigir `EditarSlot()` para usar `slotSelecionadoId`, nao `slotSelecionadoIndex`.
- [x] Em `EditarSlot()`, validar se existe slot selecionado antes de editar.
- [x] Em `EditarSlot()`, buscar o slot com `slotsEditando.Find(s => s.Id == slotSelecionadoId)`.
- [x] Corrigir `ApagarSlot()` para remover por `slotSelecionadoId`.
- [x] Em `ApagarSlot()`, limpar `slotSelecionadoId` depois de remover.
- [x] Em `ApagarSlot()`, limpar campos de atividade, inicio, fim e lugar.
- [x] Em `ApagarSlot()`, desativar botoes de editar e apagar.
- [x] Corrigir blocos vazios da barra 24h para nunca receberem selecao, outline ou conflito.
- [x] Em `CriarBlocoPreview()`, aplicar selecao/conflito apenas se `slotId` nao for vazio.
- [x] Criar estrutura de erros e avisos de slot.
- [x] Criar estrutura de erros e avisos da rotina.
- [x] Criar `ResumoDaRotina` para concentrar validacao.
- [x] Tratar `slotsUI` nulo/vazio sem executar `foreach`.
- [ ] Tratar `slotUI` nulo dentro do `foreach`.
- [ ] Evitar `Trim()`/`ToLower()` em atividade/lugar nulos.
- [ ] Preservar `Id`, erros e avisos na normalizacao de meia-noite.
- [ ] Revisar detecao de conflitos para marcar todos os slots sobrepostos.
- [ ] Definir `podeSalvarPerfil` e `podeSimular` no resumo.
- [ ] Preencher `slotsNormalizados` no resumo.
- [ ] Calcular `minutosOcupados` e `minutosVazios`.
- [ ] Detectar `rotina24hIncompleta` quando houver buracos no dia.
- [ ] Adaptar `PainelRotinas` para consumir o resumo vindo de `RotinaBase`.
- [ ] Decidir se botoes dos slots ficam em ordem de criacao ou ordem temporal.
- [ ] Sugestao inicial: mostrar botoes em ordem temporal, mas manter identificacao por `Id`.

## B. Barra 24h

- [x] Manter cores estaveis por `slot.Id`.
- [x] Mostrar conflitos em vermelho.
- [x] Mostrar slot selecionado com destaque diferente do conflito.
- [x] Confirmar que vazios aparecem proporcionalmente ao tempo.
- [x] Trocar barra 24h para desenho por posicao absoluta em vez de `HorizontalLayoutGroup`.
- [x] Remover criacao de blocos vazios na barra; o fundo da barra representa o tempo vazio.
- [ ] Revisar como representar sobreposicoes com transparencia e outline de conflito.

## C. Lugares dos slots

- [ ] Preencher `DropLugar` a partir dos tipos de predio/lugar presentes no mapa.
- [ ] Criar uma lista de lugares disponiveis reconhecidos no mapa.
- [ ] Permitir lugares temporarios quando ainda nao houver mapa importado.
- [ ] Marcar lugares temporarios como pendentes.
- [ ] Mostrar aviso quando houver lugares temporarios.
- [ ] Criar etapa de reconciliacao apos importar mapa.
- [ ] Antes da simulacao, bloquear ou alertar se ainda houver lugar temporario nao resolvido.

## D. Tipos de pessoa e populacao

- [ ] Popular dropdown de pessoas com tipos iniciais da `FabricaPessoas`, como operario e cozinheiro.
- [ ] Criar estrutura editavel de tipo de pessoa com `Id`, nome, percentual e rotina associada.
- [ ] Dividir percentuais igualmente no estado inicial.
- [ ] Mostrar percentual no input de quantidade ao selecionar tipo de pessoa.
- [ ] Permitir edicao manual do percentual.
- [ ] Definir regra para soma dos percentuais.
- [ ] Sugestao: permitir edicao incompleta durante montagem, mas exigir 100% antes da simulacao.
- [ ] Implementar botao `SalvarPessoa` para criar novo tipo ou atualizar tipo selecionado.
- [ ] Implementar exclusao de tipo de pessoa.
- [ ] Atualizar dropdown de pessoas depois de criar, editar ou deletar.

## E. Secao populacao

- [ ] Criar lista visual dos tipos de pessoa com percentual ao lado.
- [ ] Transformar cada linha da lista em botao clicavel, semelhante aos botoes de slot.
- [ ] Ao clicar em um tipo, carregar seus dados para reedicao.
- [ ] Criar grafico pizza da distribuicao populacional.
- [ ] Atualizar grafico pizza ao salvar, editar ou deletar tipo.
- [ ] Usar cor estavel por tipo de pessoa.
- [ ] Usar a lista de populacao como legenda do grafico pizza.
- [ ] Se a soma nao fechar 100%, representar ou avisar percentual restante/estouro.

## F. Rotina ligada ao tipo de pessoa

- [ ] Cada tipo de pessoa deve poder ter uma rotina associada.
- [ ] Separar rotina temporaria em edicao da rotina salva no tipo de pessoa.
- [ ] Ao selecionar tipo de pessoa, carregar sua rotina, percentual e dados no painel.
- [ ] Evitar troca de pessoa com alteracoes nao salvas sem aviso.
- [ ] Ao salvar pessoa, persistir tambem a rotina associada ou referencia para ela.

## G. Checklist antes da simulacao

- [ ] Verificar se ha conflitos de horario.
- [ ] Verificar se ha lugares temporarios nao resolvidos.
- [ ] Verificar se a soma da populacao fecha 100%.
- [ ] Verificar se todos os tipos de pessoa possuem rotina valida.
- [ ] Verificar se todos os slots possuem atividade e lugar.
- [ ] Verificar se o mapa foi importado.
- [ ] Verificar se os lugares usados nos slots existem no mapa.
- [ ] So liberar simulacao quando pendencias criticas estiverem resolvidas.

## Ordem sugerida

1. Revisar conflitos e preenchimento/cobertura das 24h.
2. Criar validacao minima dos slots.
3. Separar lugares reais do mapa e lugares temporarios.
4. Criar aviso de lugares temporarios.
5. Criar estrutura de tipos de pessoa.
6. Popular tipos iniciais.
7. Criar lista editavel da populacao.
8. Criar grafico pizza.
9. Conectar tipo de pessoa com rotina.
10. Criar checklist/bloqueio antes da simulacao.
