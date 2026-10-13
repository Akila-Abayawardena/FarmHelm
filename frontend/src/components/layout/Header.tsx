interface HeaderProps { sectionName: string }

export function Header({ sectionName }: HeaderProps) {
  return (
    <header className="app-header">
      <p className="eyebrow">Farm management workspace</p>
      <p className="header-section">{sectionName}</p>
    </header>
  )
}
