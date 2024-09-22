import unittest
import requests
import json

BASE_URL = 'http://127.0.0.1:5000'

class TestRestAPI(unittest.TestCase):

    @classmethod
    def setUpClass(cls):
        # Register a user for testing
        payload = {
            "name": "testuser",
            "age": 25,
            "email": "testuser@example.com",
            "image": "testimage.png",
            "password": "testpassword"
        }
        requests.post(f"{BASE_URL}/register", json=payload)

    def test_register(self):
        payload = {
            "name": "newuser",
            "age": 30,
            "email": "newuser@example.com",
            "image": "newimage.png",
            "password": "newpassword"
        }
        response = requests.post(f"{BASE_URL}/register", json=payload)
        self.assertEqual(response.status_code, 201)
        self.assertIn("message", response.json())

    def test_register_existing_user(self):
        payload = {
            "name": "testuser",
            "age": 25,
            "email": "testuser@example.com",
            "image": "testimage.png",
            "password": "testpassword"
        }
        response = requests.post(f"{BASE_URL}/register", json=payload)
        self.assertEqual(response.status_code, 400)

    def test_login(self):
        payload = {
            "name": "testuser",
            "password": "testpassword"
        }
        response = requests.post(f"{BASE_URL}/login", json=payload)
        self.assertEqual(response.status_code, 200)
        self.assertIn("message", response.json())

    def test_create_user_profile(self):
        payload = {
            "name": "anotheruser",
            "age": 30,
            "email": "anotheruser@example.com",
            "image": "anotherimage.png",
            "password": "anotherpassword"
        }
        response = requests.post(f"{BASE_URL}/userProfile", json=payload)
        self.assertEqual(response.status_code, 201)

    def test_get_user_profile(self):
        response = requests.get(f"{BASE_URL}/userProfile/testuser")
        self.assertEqual(response.status_code, 200)
        self.assertIn("name", response.json())

    def test_update_user_profile(self):
        payload = {
            "age": 26,
            "email": "updated@example.com",
            "image": "updated.png",
            "password": "updatedpassword"
        }
        response = requests.put(f"{BASE_URL}/userProfile/testuser", json=payload)
        self.assertEqual(response.status_code, 200)

    def test_fetch_user_profile_column(self):
        response = requests.get(f"{BASE_URL}/userProfile/testuser/email")
        self.assertEqual(response.status_code, 200)
        self.assertIn("email", response.json())

    def test_create_salary(self):
        payload = {
            "salary": 5000,
            "savings": 1000,
            "name": "testuser"
        }
        response = requests.post(f"{BASE_URL}/salaryStatements", json=payload)
        self.assertEqual(response.status_code, 201)

    def test_update_salary(self):
        payload = {
            "salary": 6000
        }
        response = requests.put(f"{BASE_URL}/salaryStatements/testuser", json=payload)
        self.assertEqual(response.status_code, 200)

    def test_fetch_salary_statement_column(self):
        response = requests.get(f"{BASE_URL}/salaryStatements/testuser/salary")
        self.assertEqual(response.status_code, 200)
        self.assertIn("salary", response.json())

    def test_update_savings(self):
        payload = {
            "savings": 1500
        }
        response = requests.put(f"{BASE_URL}/savings/testuser", json=payload)
        self.assertEqual(response.status_code, 200)

    def test_get_all_monthly_billing(self):
        params = {
            "user_name": "testuser"
        }
        response = requests.get(f"{BASE_URL}/monthlyBilling", params=params)
        self.assertEqual(response.status_code, 200)

    def test_create_monthly_billing(self):
        payload = {
            "name": "Electricity",
            "price": 150,
            "user_name": "testuser"
        }
        response = requests.post(f"{BASE_URL}/monthlyBilling", json=payload)
        self.assertEqual(response.status_code, 201)

    def test_update_monthly_billing(self):
        payload = {
            "price": 200,
            "user_name": "testuser"
        }
        response = requests.put(f"{BASE_URL}/monthlyBilling/Electricity-testuser", json=payload)
        self.assertEqual(response.status_code, 200)

    def test_delete_monthly_billing(self):
        response = requests.delete(f"{BASE_URL}/monthlyBilling/Electricity-testuser")
        self.assertEqual(response.status_code, 200)

    def test_create_bill(self):
        payload = {
            "date": "2023-01-01",
            "name": "Water",
            "price": 50,
            "user_name": "testuser"
        }
        response = requests.post(f"{BASE_URL}/bills", json=payload)
        self.assertEqual(response.status_code, 201)

    def test_update_bill(self):
        payload = {
            "name": "Water",
            "date": "2023-01-01",
            "price": 60,
            "user_name": "testuser"
        }
        response = requests.put(f"{BASE_URL}/bills/Water-testuser-2023-01-01-50", json=payload)
        self.assertEqual(response.status_code, 200)

    def test_delete_bill(self):
        response = requests.delete(f"{BASE_URL}/bills/Water-testuser-2023-01-01-60")
        self.assertEqual(response.status_code, 200)

    def test_get_all_bills(self):
        params = {
            "user_name": "testuser"
        }
        response = requests.get(f"{BASE_URL}/bills", params=params)
        self.assertEqual(response.status_code, 200)

    def test_get_bills_in_date_range(self):
        params = {
            "user_name": "testuser",
            "start_date": "2023-01-01",
            "end_date": "2023-12-31"
        }
        response = requests.get(f"{BASE_URL}/bills/range", params=params)
        self.assertEqual(response.status_code, 200)

if __name__ == '__main__':
    unittest.main()
