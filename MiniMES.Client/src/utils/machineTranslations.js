export function translateMachineType(type) {
  if (!type) return '-';
  const map = {
    'CNC Mill': 'CNC fréza',
    'CNC Milling Machine': 'CNC fréza',
    'CNC Lathe': 'CNC soustruh',
    'Welding Robot': 'Robotický svářeč',
    'Assembly': 'Montáž',
    'Assembly Station': 'Montážní stanice',
    'Assembly Line': 'Montážní linka',
    '3D Printer': '3D tiskárna',
    'Hydraulic Press': 'Hydraulický lis',
    'Press': 'Lis',
    'Laser Cutter': 'Laserová řezačka',
    'Waterjet Cutter': 'Vodní paprsková řezačka',
    'CNC Router': 'CNC frézka (router)',
    'Grinder': 'Bruska',
    'Drill Press': 'Vrtací lis',
    'Saw': 'Pila',
    'Stamping Press': 'Lis na tváření',
    'Injection Molder': 'Vstřikolis',
    'Injection Molding Machine': 'Vstřikolis',
    'Packaging Machine': 'Balička',
    'Paint Booth': 'Lakovací kabina',
    'Heat Treatment Oven': 'Žíhací pec',
    'Quality Inspection Station': 'Stanice kontroly kvality'
  };
  return map[type] || type;
}

export function translateLocation(location) {
  if (!location) return '-';
  const map = {
    'Production Hall A': 'Výrobní hala A',
    'Production Hall B': 'Výrobní hala B',
    'Production Hall C': 'Výrobní hala C',
    'Warehouse': 'Sklad',
    'Main Workshop': 'Hlavní dílna',
    'Maintenance Area': 'Údržbová zóna',
    'Assembly Line 1': 'Montážní linka 1',
    'Assembly Line 2': 'Montážní linka 2'
  };
  return map[location] || location;
}

export function translateMachineName(name) {
  if (!name) return '-';
  const patterns = [
    { re: /^Robotic Welder(.*)$/i, repl: 'Robotický svářeč$1' },
    { re: /^Welding Robot(.*)$/i, repl: 'Robotický svářeč$1' },
    { re: /^CNC Milling Machine(.*)$/i, repl: 'CNC fréza$1' },
    { re: /^CNC Mill(.*)$/i, repl: 'CNC fréza$1' },
    { re: /^CNC Lathe Machine(.*)$/i, repl: 'CNC soustruh$1' },
    { re: /^CNC Lathe(.*)$/i, repl: 'CNC soustruh$1' },
    { re: /^Assembly Station(.*)$/i, repl: 'Montážní stanice$1' },
    { re: /^Assembly Line(.*)$/i, repl: 'Montážní linka$1' },
    { re: /^Assembly(.*)$/i, repl: 'Montáž$1' },
    { re: /^3D Printer(.*)$/i, repl: '3D tiskárna$1' },
    { re: /^Hydraulic Press(.*)$/i, repl: 'Hydraulický lis$1' },
    { re: /^Press(.*)$/i, repl: 'Lis$1' },
    { re: /^Laser Cutter(.*)$/i, repl: 'Laserová řezačka$1' },
    { re: /^Waterjet Cutter(.*)$/i, repl: 'Vodní paprsková řezačka$1' },
    { re: /^CNC Router(.*)$/i, repl: 'CNC frézka (router)$1' },
    { re: /^Grinder(.*)$/i, repl: 'Bruska$1' },
    { re: /^Drill Press(.*)$/i, repl: 'Vrtací lis$1' },
    { re: /^Saw(.*)$/i, repl: 'Pila$1' },
    { re: /^Stamping Press(.*)$/i, repl: 'Lis na tváření$1' },
    { re: /^Injection Molder(.*)$/i, repl: 'Vstřikolis$1' },
    { re: /^Injection Molding Machine(.*)$/i, repl: 'Vstřikolis$1' },
    { re: /^Packaging Machine(.*)$/i, repl: 'Balička$1' },
    { re: /^Paint Booth(.*)$/i, repl: 'Lakovací kabina$1' },
    { re: /^Heat Treatment Oven(.*)$/i, repl: 'Žíhací pec$1' },
    { re: /^Quality Inspection Station(.*)$/i, repl: 'Stanice kontroly kvality$1' },
    { re: /^Test Machine(.*)$/i, repl: 'Testovací stroj$1' }
  ];
  for (const { re, repl } of patterns) {
    if (re.test(name)) {
      return name.replace(re, repl).trim();
    }
  }
  return name;
}

export function translateMachineNote(note) {
  if (!note) return '-';
  const map = {
    'Main production machine': 'Hlavní výrobní stroj',
    'Backup unit': 'Záložní jednotka',
    'Requires maintenance': 'Vyžaduje údržbu',
    'Scheduled maintenance': 'Plánovaná údržba',
    'Calibrated': 'Kalibrováno',
    'Under inspection': 'Probíhá kontrola',
    'High precision': 'Vysoká přesnost',
    'Prototype line': 'Prototypová linka',
    'Training station': 'Výuková stanice'
  };
  return map[note] || note;
}
