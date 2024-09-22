from flask import Flask, request, jsonify
from flask_cors import CORS
import psycopg2
from psycopg2.extras import RealDictCursor
from psycopg2 import IntegrityError
import logging
import bcrypt

app = Flask(__name__)
CORS(app)  # Enable CORS for all routes

# Set up logging
logging.basicConfig(level=logging.DEBUG)

# Database connection
def get_db_connection():
    conn = psycopg2.connect(
        database="MoneyTracking",
        user="postgres",
        password="Emilemil2002",
        host="localhost",
        port="5432"
    )
    return conn

def is_valid_username(username):
    return ' ' not in username

# Register a new user
@app.route('/register', methods=['POST'])
def register():
    data = request.json
    app.logger.debug('Received data for registration: %s', data)
    name = data.get('name')
    age = data.get('age')
    email = data.get('email')
    image = data.get('image')
    password = data.get('password')

    if not is_valid_username(name):
        return jsonify({'message': 'Username must not contain spaces'}), 400

    hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt())
    
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'INSERT INTO public."userProfile" (name, age, email, image, password) VALUES (%s, %s, %s, %s, %s)',
            (name, age, email, image, hashed_password.decode('utf-8'))
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'User registered successfully'}), 201
    except IntegrityError as e:
        conn.rollback()
        app.logger.error('Error registering user: %s', e)
        return jsonify({'message': 'Failed to register user', 'error': 'Username already exists'}), 400
    except Exception as e:
        app.logger.error('Error registering user: %s', e)
        return jsonify({'message': 'Failed to register user', 'error': str(e)}), 500

# Login a user
@app.route('/login', methods=['POST'])
def login():
    data = request.json
    app.logger.debug('Received data for login: %s', data)
    name = data.get('name')
    password = data.get('password')

    if not is_valid_username(name):
        return jsonify({'message': 'Username must not contain spaces'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute('SELECT password FROM public."userProfile" WHERE name = %s', (name,))
        user = cursor.fetchone()
        cursor.close()
        conn.close()
        
        if user is None:
            return jsonify({'message': 'User does not exist'}), 401
        elif bcrypt.checkpw(password.encode('utf-8'), user['password'].encode('utf-8')):
            return jsonify({'message': 'Login successful'}), 200
        else:
            return jsonify({'message': 'Incorrect password'}), 401
    except Exception as e:
        app.logger.error('Error logging in user: %s', e)
        return jsonify({'message': 'Failed to login user', 'error': str(e)}), 500

# Create user profile
@app.route('/userProfile', methods=['POST'])
def create_user_profile():
    data = request.json
    app.logger.debug('Received data: %s', data)
    name = data.get('name')
    age = data.get('age')
    email = data.get('email')
    image = data.get('image')
    password = data.get('password')

    if not is_valid_username(name):
        return jsonify({'message': 'Username must not contain spaces'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'INSERT INTO public."userProfile" (name, age, email, image, password) VALUES (%s, %s, %s, %s, %s)',
            (name, age, email, image, password)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'User profile created successfully'}), 201
    except IntegrityError as e:
        conn.rollback()
        app.logger.error('Error creating user profile: %s', e)
        return jsonify({'message': 'Failed to create user profile', 'error': 'Username already exists'}), 400
    except Exception as e:
        app.logger.error('Error creating user profile: %s', e)
        return jsonify({'message': 'Failed to create user profile', 'error': str(e)}), 500

# Fetch user profile
@app.route('/userProfile/<name>', methods=['GET'])
def get_user_profile(name):
    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute('SELECT name, age, email, image FROM public."userProfile" WHERE name = %s', (name,))
        user = cursor.fetchone()
        cursor.close()
        conn.close()
        
        if user:
            return jsonify(user), 200
        else:
            return jsonify({'message': 'User not found'}), 404
    except Exception as e:
        app.logger.error('Error fetching user profile: %s', e)
        return jsonify({'message': 'Failed to fetch user profile', 'error': str(e)}), 500

# Update user profile
@app.route('/userProfile/<name>', methods=['PUT'])
def update_user_profile(name):
    data = request.json
    app.logger.debug('Received data: %s', data)
    age = data.get('age')
    email = data.get('email')
    image = data.get('image')
    password = data.get('password')

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'UPDATE public."userProfile" SET age = %s, email = %s, image = %s, password = %s WHERE name = %s',
            (age, email, image, password, name)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'User profile updated successfully'}), 200
    except Exception as e:
        app.logger.error('Error updating user profile: %s', e)
        return jsonify({'message': 'Failed to update user profile', 'error': str(e)}), 500

# Fetch specific column from user profile
@app.route('/userProfile/<name>/<column>', methods=['GET'])
def fetch_user_profile_column(name, column):
    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute(f'SELECT {column} FROM public."userProfile" WHERE name = %s', (name,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return jsonify(result), 200
        else:
            return jsonify({'message': 'No data found'}), 404
    except Exception as e:
        app.logger.error('Error fetching column from user profile: %s', e)
        return jsonify({'message': 'Failed to fetch column', 'error': str(e)}), 500

# Create salary for user
@app.route('/salaryStatements', methods=['POST'])
def create_salary():
    data = request.json
    app.logger.debug('Received data: %s', data)
    salary = data.get('salary')
    savings = data.get('savings')
    name = data.get('name')

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'INSERT INTO public."salaryStatements" (salary, savings, name) VALUES (%s, %s, %s)',
            (salary, savings, name)
        )
        conn.commit()
        cursor.close()
        conn.close()
        app.logger.debug('Salary created successfully')
        return jsonify({'message': 'Salary created successfully'}), 201
    except Exception as e:
        app.logger.error('Error creating salary: %s', e)
        return jsonify({'message': 'Failed to create salary', 'error': str(e)}), 500

# Update salary for user
@app.route('/salaryStatements/<name>', methods=['PUT'])
def update_salary(name):
    data = request.json
    app.logger.debug('Received data: %s', data)
    salary = data.get('salary')

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'UPDATE public."salaryStatements" SET salary = %s WHERE name = %s',
            (salary, name)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Salary updated successfully'}), 200
    except Exception as e:
        app.logger.error('Error updating salary: %s', e)
        return jsonify({'message': 'Failed to update salary', 'error': str(e)}), 500

# Fetch specific column from salary statements
@app.route('/salaryStatements/<name>/<column>', methods=['GET'])
def fetch_salary_statement_column(name, column):
    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute(f'SELECT {column} FROM public."salaryStatements" WHERE name = %s', (name,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return jsonify(result), 200
        else:
            return jsonify({'message': 'No data found'}), 404
    except Exception as e:
        app.logger.error('Error fetching column from salary statements: %s', e)
        return jsonify({'message': 'Failed to fetch column', 'error': str(e)}), 500

# Update savings for user
@app.route('/savings/<name>', methods=['PUT'])
def update_savings(name):
    data = request.json
    app.logger.debug('Received data: %s', data)
    savings = data.get('savings')

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'UPDATE public."salaryStatements" SET savings = %s WHERE name = %s',
            (savings, name)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Savings updated successfully'}), 200
    except Exception as e:
        app.logger.error('Error updating savings: %s', e)
        return jsonify({'message': 'Failed to update savings', 'error': str(e)}), 500

# Fetch all monthly billing entries for a specific user
@app.route('/monthlyBilling', methods=['GET'])
def get_all_monthly_billing():
    user_name = request.args.get('user_name')
    if not user_name:
        return jsonify({'message': 'User name is required'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute('SELECT name, price, user_name, "ID" FROM public."monthlyBilling" WHERE user_name = %s', (user_name,))
        billing_entries = cursor.fetchall()
        cursor.close()
        conn.close()
        return jsonify(billing_entries), 200
    except Exception as e:
        app.logger.error('Error fetching monthly billing entries: %s', e)
        return jsonify({'message': 'Failed to fetch monthly billing entries', 'error': str(e)}), 500

# Create monthly billing entry
@app.route('/monthlyBilling', methods=['POST'])
def create_monthly_billing():
    data = request.json
    app.logger.debug('Received data: %s', data)
    name = data.get('name')
    price = data.get('price')
    user_name = data.get('user_name')
    id = f"{name}-{user_name}"

    if not name or not price or not user_name:
        return jsonify({'message': 'All fields are required'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'INSERT INTO public."monthlyBilling" (name, price, user_name, "ID") VALUES (%s, %s, %s, %s)',
            (name, price, user_name, id)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Monthly billing entry created successfully'}), 201
    except IntegrityError as e:
        conn.rollback()
        app.logger.error('Error creating monthly billing entry: %s', e)
        return jsonify({'message': 'Failed to create monthly billing entry', 'error': 'ID already exists'}), 400
    except Exception as e:
        app.logger.error('Error creating monthly billing entry: %s', e)
        return jsonify({'message': 'Failed to create monthly billing entry', 'error': str(e)}), 500

# Update monthly billing entry by ID
@app.route('/monthlyBilling/<id>', methods=['PUT'])
def update_monthly_billing(id):
    data = request.json
    app.logger.debug('Received data: %s', data)
    price = data.get('price')
    user_name = data.get('user_name')

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'UPDATE public."monthlyBilling" SET price = %s, user_name = %s WHERE "ID" = %s',
            (price, user_name, id)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Monthly billing entry updated successfully'}), 200
    except Exception as e:
        app.logger.error('Error updating monthly billing entry: %s', e)
        return jsonify({'message': 'Failed to update monthly billing entry', 'error': str(e)}), 500

# Delete monthly billing entry by ID
@app.route('/monthlyBilling/<id>', methods=['DELETE'])
def delete_monthly_billing(id):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute('DELETE FROM public."monthlyBilling" WHERE "ID" = %s', (id,))
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Monthly billing entry deleted successfully'}), 200
    except Exception as e:
        app.logger.error('Error deleting monthly billing entry: %s', e)
        return jsonify({'message': 'Failed to delete monthly billing entry', 'error': str(e)}), 500

# Fetch specific column from monthly billing
@app.route('/monthlyBilling/<id>/<column>', methods=['GET'])
def fetch_monthly_billing_column(id, column):
    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute(f'SELECT {column} FROM public."monthlyBilling" WHERE "ID" = %s', (id,))
        result = cursor.fetchone()
        cursor.close()
        conn.close()
        if result:
            return jsonify(result), 200
        else:
            return jsonify({'message': 'No data found'}), 404
    except Exception as e:
        app.logger.error('Error fetching column from monthly billing: %s', e)
        return jsonify({'message': 'Failed to fetch column', 'error': str(e)}), 500

def generate_id(name, user_name, date, price):
    return f"{name}-{user_name}-{date}-{price}"

# Delete bill entry by ID
@app.route('/bills/<id>', methods=['DELETE'])
def delete_bill(id):
    try:
        name, user_name, date, price = id.rsplit('-', 3)
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'DELETE FROM public.bills WHERE "ID" = %s',
            (id,)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Bill entry deleted successfully'}), 200
    except Exception as e:
        app.logger.error('Error deleting bill entry: %s', e)
        return jsonify({'message': 'Failed to delete bill entry', 'error': str(e)}), 500

# Update bill entry by ID
@app.route('/bills/<id>', methods=['PUT'])
def update_bill(id):
    data = request.json
    app.logger.debug('Received data: %s', data)
    old_name, old_user_name, old_date, old_price = id.rsplit('-', 3)
    new_name = data.get('name')
    new_date = data.get('date')[:10]  # Ensure date format is correct
    new_price = data.get('price')
    new_user_name = data.get('user_name')
    new_id = generate_id(new_name, new_user_name, new_date, new_price)

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'UPDATE public.bills SET name = %s, date = %s, price = %s, user_name = %s, "ID" = %s WHERE "ID" = %s',
            (new_name, new_date, new_price, new_user_name, new_id, id)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Bill entry updated successfully'}), 200
    except Exception as e:
        app.logger.error('Error updating bill entry: %s', e)
        return jsonify({'message': 'Failed to update bill entry', 'error': str(e)}), 500

# Create bill entry
@app.route('/bills', methods=['POST'])
def create_bill():
    data = request.json
    app.logger.debug('Received data: %s', data)
    date = data.get('date')[:10]  # Ensure date format is correct
    name = data.get('name')
    price = data.get('price')
    user_name = data.get('user_name')
    id = generate_id(name, user_name, date, price)

    if not name or not price or not user_name or not date:
        return jsonify({'message': 'All fields are required'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            'INSERT INTO public.bills (date, name, price, user_name, "ID") VALUES (%s, %s, %s, %s, %s)',
            (date, name, price, user_name, id)
        )
        conn.commit()
        cursor.close()
        conn.close()
        return jsonify({'message': 'Bill entry created successfully', 'id': id}), 201
    except Exception as e:
        app.logger.error('Error creating bill entry: %s', e)
        return jsonify({'message': 'Failed to create bill entry', 'error': str(e)}), 500

# Fetch all bills for a specific user
@app.route('/bills', methods=['GET'])
def get_all_bills():
    user_name = request.args.get('user_name')
    if not user_name:
        return jsonify({'message': 'User name is required'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute('SELECT name, price, date, user_name, "ID" FROM public.bills WHERE user_name = %s', (user_name,))
        bills = cursor.fetchall()
        cursor.close()
        conn.close()
        return jsonify(bills), 200
    except Exception as e:
        app.logger.error('Error fetching bills: %s', e)
        return jsonify({'message': 'Failed to fetch bills', 'error': str(e)}), 500
    
@app.route('/bills/range', methods=['GET'])
def get_bills_in_date_range():
    user_name = request.args.get('user_name')
    start_date = request.args.get('start_date')
    end_date = request.args.get('end_date')

    if not user_name:
        return jsonify({'message': 'User name is required'}), 400
    if not start_date or not end_date:
        return jsonify({'message': 'Start date and end date are required'}), 400

    try:
        conn = get_db_connection()
        cursor = conn.cursor(cursor_factory=RealDictCursor)
        cursor.execute(
            'SELECT name, price, date, user_name, "ID" FROM public.bills WHERE user_name = %s AND date BETWEEN %s AND %s',
            (user_name, start_date, end_date)
        )
        bills = cursor.fetchall()
        cursor.close()
        conn.close()
        return jsonify(bills), 200
    except Exception as e:
        app.logger.error('Error fetching bills: %s', e)
        return jsonify({'message': 'Failed to fetch bills', 'error': str(e)}), 500


if __name__ == '__main__':
    app.run(debug=True)