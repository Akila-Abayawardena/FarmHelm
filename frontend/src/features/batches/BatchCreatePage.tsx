import { type FormEvent, useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { createBatch } from '../../api/batches/batchApi'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import type { Crop, CropStage, Farm, FarmLocation, Variety } from '../../api/masterData/masterDataTypes'
import { FormError } from '../../components/feedback/FormError'
import { errorMessage } from '../../components/feedback/errorMessage'

function today(): string {
  const date = new Date()
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

export function BatchCreatePage() {
  const navigate = useNavigate()
  const [farms, setFarms] = useState<Farm[]>()
  const [locations, setLocations] = useState<FarmLocation[]>([])
  const [crops, setCrops] = useState<Crop[]>([])
  const [varieties, setVarieties] = useState<Variety[]>([])
  const [stages, setStages] = useState<CropStage[]>([])
  const [farmId, setFarmId] = useState('')
  const [cropId, setCropId] = useState('')
  const [form, setForm] = useState({ varietyId: '', locationId: '', initialStageId: '', plantingDate: today(), initialPlantCount: 1, individualTrackingEnabled: false, notes: '' })
  const [error, setError] = useState<string>()
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    const controller = new AbortController()
    void masterDataApi.listFarms(controller.signal).then((items) => {
      setFarms(items)
      setFarmId(items[0]?.id ?? '')
    }).catch((requestError) => { if (!controller.signal.aborted) { setError(errorMessage(requestError)); setFarms([]) } })
    return () => controller.abort()
  }, [])

  useEffect(() => {
    if (!farmId) { setLocations([]); setCrops([]); setCropId(''); return }
    const controller = new AbortController()
    setForm((current) => ({ ...current, locationId: '', varietyId: '', initialStageId: '' }))
    void Promise.all([masterDataApi.listLocations(farmId, false, controller.signal), masterDataApi.listCrops(farmId, false, controller.signal)])
      .then(([nextLocations, nextCrops]) => { setLocations(nextLocations); setCrops(nextCrops); setCropId(nextCrops[0]?.id ?? '') })
      .catch((requestError) => { if (!controller.signal.aborted) setError(errorMessage(requestError)) })
    return () => controller.abort()
  }, [farmId])

  useEffect(() => {
    if (!cropId) { setVarieties([]); setStages([]); return }
    const controller = new AbortController()
    setForm((current) => ({ ...current, varietyId: '', initialStageId: '' }))
    void Promise.all([masterDataApi.listVarieties(cropId, false, controller.signal), masterDataApi.listCropStages(cropId, false, controller.signal)])
      .then(([nextVarieties, nextStages]) => {
        setVarieties(nextVarieties); setStages(nextStages)
        setForm((current) => ({ ...current, varietyId: nextVarieties[0]?.id ?? '', initialStageId: nextStages[0]?.id ?? '' }))
      })
      .catch((requestError) => { if (!controller.signal.aborted) setError(errorMessage(requestError)) })
    return () => controller.abort()
  }, [cropId])

  async function submit(event: FormEvent) {
    event.preventDefault()
    if (!farmId || !form.varietyId || !form.initialStageId) return
    setSubmitting(true); setError(undefined)
    try {
      const created = await createBatch({ farmId, varietyId: form.varietyId, locationId: form.locationId || null, plantingDate: form.plantingDate, initialPlantCount: form.initialPlantCount, initialStageId: form.initialStageId, individualTrackingEnabled: form.individualTrackingEnabled, notes: form.notes || undefined })
      if (!created) throw new Error('The batch endpoint returned no response body.')
      navigate(`/batches/${created.batchId}`)
    } catch (requestError) { setError(errorMessage(requestError)) } finally { setSubmitting(false) }
  }

  const hasSetup = farms !== undefined && farms.length > 0
  const canCreate = Boolean(farmId && cropId && form.varietyId && form.initialStageId)
  return <section className="page" aria-labelledby="create-batch-title">
    <p className="eyebrow">Agricultural core</p><h1 id="create-batch-title">Create batch</h1><p className="page-description">Record a new planting batch using the active setup data for its farm and crop.</p><FormError error={error} />
    {farms === undefined && <p className="batch-state" role="status">Loading setup data…</p>}
    {farms?.length === 0 && <section className="setup-panel"><p>Create a farm before recording a batch.</p><Link className="text-link" to="/farms">Set up farms</Link></section>}
    {hasSetup && <form className="setup-panel form-grid" onSubmit={submit}>
      <label>Farm<select required value={farmId} onChange={(event) => setFarmId(event.target.value)}>{farms.map((farm) => <option key={farm.id} value={farm.id}>{farm.farmCode} — {farm.name}</option>)}</select></label>
      <label>Crop<select required value={cropId} onChange={(event) => setCropId(event.target.value)} disabled={crops.length === 0}>{crops.length === 0 ? <option>No active crops available</option> : crops.map((crop) => <option key={crop.id} value={crop.id}>{crop.cropCode} — {crop.name}</option>)}</select></label>
      {crops.length === 0 && <p className="form-wide helper-text">This farm needs an active crop, variety, and stage before a batch can be created. <Link to="/crops">Set up crops</Link></p>}
      {crops.length > 0 && <><label>Variety<select required value={form.varietyId} onChange={(event) => setForm({ ...form, varietyId: event.target.value })} disabled={varieties.length === 0}>{varieties.length === 0 ? <option>No active varieties available</option> : varieties.map((variety) => <option key={variety.id} value={variety.id}>{variety.varietyCode} — {variety.name}</option>)}</select></label>
      <label>Initial stage<select required value={form.initialStageId} onChange={(event) => setForm({ ...form, initialStageId: event.target.value })} disabled={stages.length === 0}>{stages.length === 0 ? <option>No active stages available</option> : stages.map((stage) => <option key={stage.id} value={stage.id}>{stage.displayOrder} — {stage.name}</option>)}</select></label>
      <label>Location (optional)<select value={form.locationId} onChange={(event) => setForm({ ...form, locationId: event.target.value })}><option value="">No location assigned</option>{locations.map((location) => <option key={location.id} value={location.id}>{location.locationCode} — {location.name}</option>)}</select></label>
      <label>Planting date<input required type="date" value={form.plantingDate} onChange={(event) => setForm({ ...form, plantingDate: event.target.value })} /></label>
      <label>Initial plant count<input required type="number" min="1" value={form.initialPlantCount} onChange={(event) => setForm({ ...form, initialPlantCount: Number(event.target.value) })} /></label>
      <label className="checkbox-label"><input type="checkbox" checked={form.individualTrackingEnabled} onChange={(event) => setForm({ ...form, individualTrackingEnabled: event.target.checked })} /> Track individual plants</label>
      <label className="form-wide">Notes<textarea value={form.notes} onChange={(event) => setForm({ ...form, notes: event.target.value })} /></label>
      <button className="button button-primary" disabled={!canCreate || submitting}>{submitting ? 'Creating…' : 'Create batch'}</button></>}
    </form>}
  </section>
}
