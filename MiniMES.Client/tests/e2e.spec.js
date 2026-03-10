import { test, expect } from '@playwright/test'

const API_BASE = 'http://localhost:5000'
let token = null

// Mock login token
test.beforeAll(async () => {
  const response = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username: 'admin', password: 'admin' })
  })
  const data = await response.json()
  token = data.token
})

test('User can login', async ({ page }) => {
  await page.goto('/login')
  
  await expect(page.getByRole('heading', { name: 'Mini-MES Login' })).toBeVisible()
  
  await page.fill('input[name="username"]', 'admin')
  await page.fill('input[name="password"]', 'admin')
  await page.click('button:has-text("Login")')
  
  await page.waitForURL('/dashboard')
  await expect(page.getByRole('heading', { name: /Dashboard/i })).toBeVisible()
})

test('Dashboard displays machines and work orders', async ({ page }) => {
  await page.goto('/dashboard')
  
  // Wait for API calls to complete
  await page.waitForLoadState('networkidle')
  
  await expect(page.getByRole('heading', { name: /Dashboard/i })).toBeVisible()
  await expect(page.getByText(/Machines|Work Orders|Production Logs/i)).toBeVisible()
})

test('Can navigate to work orders page', async ({ page }) => {
  await page.goto('/work-orders')
  
  await page.waitForLoadState('networkidle')
  
  await expect(page.getByRole('heading', { name: /Work Orders/i })).toBeVisible()
  await expect(page.getByRole('button', { name: /Create|New/i })).toBeVisible()
})

test('Can navigate to machines page', async ({ page }) => {
  await page.goto('/machines')
  
  await page.waitForLoadState('networkidle')
  
  await expect(page.getByRole('heading', { name: /Machines/i })).toBeVisible()
})

test('Navigation menu is accessible', async ({ page }) => {
  await page.goto('/dashboard')
  
  // Check if navigation links exist
  await expect(page.getByRole('link', { name: /Dashboard/i })).toBeVisible()
  await expect(page.getByRole('link', { name: /Work Orders|Orders/i })).toBeVisible()
  await expect(page.getByRole('link', { name: /Machines/i })).toBeVisible()
  
  // Test navigation
  await page.click('text=/Work Orders|Orders/')
  await page.waitForLoadState('networkidle')
  await expect(page).toHaveURL(/work-orders|orders/)
})
