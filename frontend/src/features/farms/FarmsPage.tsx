import { type FormEvent, useEffect, useState } from 'react'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import type { Farm, FarmLocation, MortalityReason } from '../../api/masterData/masterDataTypes'
import { FormError } from '../../components/feedback/FormError'
import { errorMessage } from '../../components/feedback/errorMessage'

const initialFarm = { name: '', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: '' }

export function FarmsPage() {
  const [farms, setFarms] = useState<Farm[]>()
  const [selectedFarmId, setSelectedFarmId] = useState('')
  const [locations, setLocations] = useState<FarmLocation[]>()
  const [reasons, setReasons] = useState<MortalityReason[]>()
  const [farmForm, setFarmForm] = useState(initialFarm)
  const [locationForm, setLocationForm] = useState({ name: '', description: '' })
  const [reasonForm, setReasonForm] = useState({ name: '', displayOrder: 1 })
  const [error, setError] = useState<string>()
  const [submitting, setSubmitting] = useState('')
  const selectedFarm = farms?.find((farm) => farm.id === selectedFarmId)

  async function loadFarms() {
    try {
      const next = await masterDataApi.listFarms()
      setFarms(next)
      setSelectedFarmId((current) => current && next.some((farm) => farm.id === current) ? current : (next[0]?.id ?? ''))
    } catch (requestError) { setError(errorMessage(requestError)); setFarms([]) }
  }
  async function loadChildren(farmId: string) {
    if (!farmId) { setLocations([]); setReasons([]); return }
    try {
      const [nextLocations, nextReasons] = await Promise.all([masterDataApi.listLocations(farmId), masterDataApi.listMortalityReasons(farmId)])
      setLocations(nextLocations); setReasons(nextReasons)
      setReasonForm((form) => ({ ...form, displayOrder: (nextReasons.at(-1)?.displayOrder ?? 0) + 1 }))
    } catch (requestError) { setError(errorMessage(requestError)) }
  }
  useEffect(() => { void loadFarms() }, [])
  useEffect(() => { void loadChildren(selectedFarmId) }, [selectedFarmId])

  async function submitFarm(event: FormEvent) {
    event.preventDefault(); setSubmitting('farm'); setError(undefined)
    try { const created = await masterDataApi.createFarm({ ...farmForm, notes: farmForm.notes || undefined }); setFarmForm(initialFarm); await loadFarms(); setSelectedFarmId(created?.farmId ?? '') }
    catch (requestError) { setError(errorMessage(requestError)) } finally { setSubmitting('') }
  }
  async function submitLocation(event: FormEvent) {
    event.preventDefault(); if (!selectedFarmId) return; setSubmitting('location'); setError(undefined)
    try { await masterDataApi.createLocation(selectedFarmId, { ...locationForm, description: locationForm.description || undefined }); setLocationForm({ name: '', description: '' }); await loadChildren(selectedFarmId) }
    catch (requestError) { setError(errorMessage(requestError)) } finally { setSubmitting('') }
  }
  async function submitReason(event: FormEvent) {
    event.preventDefault(); if (!selectedFarmId) return; setSubmitting('reason'); setError(undefined)
    try { await masterDataApi.createMortalityReason(selectedFarmId, reasonForm); setReasonForm({ name: '', displayOrder: reasonForm.displayOrder + 1 }); await loadChildren(selectedFarmId) }
    catch (requestError) { setError(errorMessage(requestError)) } finally { setSubmitting('') }
  }

  return <section className="page" aria-labelledby="farms-title">
    <p className="eyebrow">Agricultural core</p><h1 id="farms-title">Farms</h1><p className="page-description">Set up farms, growing locations, and mortality reasons.</p><FormError error={error} />
    <div className="setup-grid"><section className="setup-panel"><h2>Create farm</h2><form onSubmit={submitFarm} className="form-grid"><label>Name<input required value={farmForm.name} onChange={(event) => setFarmForm({ ...farmForm, name: event.target.value })} /></label><label>Default currency<input required maxLength={3} value={farmForm.defaultCurrency} onChange={(event) => setFarmForm({ ...farmForm, defaultCurrency: event.target.value })} /></label><label>Time zone<input required value={farmForm.timeZone} onChange={(event) => setFarmForm({ ...farmForm, timeZone: event.target.value })} /></label><label className="form-wide">Notes<textarea value={farmForm.notes} onChange={(event) => setFarmForm({ ...farmForm, notes: event.target.value })} /></label><button className="button button-primary" disabled={submitting === 'farm'}>{submitting === 'farm' ? 'Creating…' : 'Create farm'}</button></form></section>
      <section className="setup-panel"><h2>Select farm</h2>{farms === undefined && <p role="status">Loading farms…</p>}{farms?.length === 0 && <p>No farms have been created yet. Create your first farm to begin setup.</p>}{farms && farms.length > 0 && <label>Farm<select value={selectedFarmId} onChange={(event) => setSelectedFarmId(event.target.value)}>{farms.map((farm) => <option key={farm.id} value={farm.id}>{farm.farmCode} — {farm.name}</option>)}</select></label>}{selectedFarm && <dl className="detail-list"><div><dt>Farm code</dt><dd>{selectedFarm.farmCode}</dd></div><div><dt>Currency</dt><dd>{selectedFarm.defaultCurrency}</dd></div><div><dt>Time zone</dt><dd>{selectedFarm.timeZone}</dd></div></dl>}</section></div>
    {selectedFarm && <div className="setup-grid"><section className="setup-panel"><h2>Locations</h2><ul className="simple-list">{locations?.map((location) => <li key={location.id}><strong>{location.locationCode}</strong> {location.name}</li>)}{locations?.length === 0 && <li>No locations created.</li>}</ul><form onSubmit={submitLocation} className="form-grid"><label>Name<input required value={locationForm.name} onChange={(event) => setLocationForm({ ...locationForm, name: event.target.value })} /></label><label>Description<input value={locationForm.description} onChange={(event) => setLocationForm({ ...locationForm, description: event.target.value })} /></label><button className="button button-secondary" disabled={submitting === 'location'}>{submitting === 'location' ? 'Creating…' : 'Add location'}</button></form></section>
      <section className="setup-panel"><h2>Mortality reasons</h2><ul className="simple-list">{reasons?.map((reason) => <li key={reason.id}><strong>{reason.displayOrder}</strong> {reason.name}</li>)}{reasons?.length === 0 && <li>No mortality reasons created.</li>}</ul><form onSubmit={submitReason} className="form-grid"><label>Name<input required value={reasonForm.name} onChange={(event) => setReasonForm({ ...reasonForm, name: event.target.value })} /></label><label>Display order<input required type="number" min="0" value={reasonForm.displayOrder} onChange={(event) => setReasonForm({ ...reasonForm, displayOrder: Number(event.target.value) })} /></label><button className="button button-secondary" disabled={submitting === 'reason'}>{submitting === 'reason' ? 'Creating…' : 'Add reason'}</button></form></section></div>}
  </section>
}
