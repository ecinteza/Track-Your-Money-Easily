--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: update_id(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_id() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW."ID" = NEW.name || '-' || NEW.user_name;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_id() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: bills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.bills (
    date date,
    name text,
    price integer,
    user_name text,
    "ID" text NOT NULL
);


ALTER TABLE public.bills OWNER TO postgres;

--
-- Name: monthlyBilling; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."monthlyBilling" (
    name text NOT NULL,
    price integer,
    user_name text,
    "ID" text NOT NULL
);


ALTER TABLE public."monthlyBilling" OWNER TO postgres;

--
-- Name: salaryStatements; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."salaryStatements" (
    salary integer,
    savings integer,
    name text
);


ALTER TABLE public."salaryStatements" OWNER TO postgres;

--
-- Name: userProfile; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."userProfile" (
    name text,
    age integer,
    email text,
    image text,
    password text
);


ALTER TABLE public."userProfile" OWNER TO postgres;


--
-- Name: bills bills_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.bills
    ADD CONSTRAINT bills_pkey PRIMARY KEY ("ID");


--
-- Name: monthlyBilling monthlyBilling_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."monthlyBilling"
    ADD CONSTRAINT "monthlyBilling_pkey" PRIMARY KEY ("ID");


--
-- Name: userProfile unique_user_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."userProfile"
    ADD CONSTRAINT unique_user_name UNIQUE (name);


--
-- Name: monthlyBilling update_id_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_id_trigger BEFORE INSERT OR UPDATE ON public."monthlyBilling" FOR EACH ROW EXECUTE FUNCTION public.update_id();


--
-- PostgreSQL database dump complete
--

--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: update_id(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_id() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW."ID" = NEW.name || '-' || NEW.user_name;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_id() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: bills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.bills (
    date date,
    name text,
    price integer,
    user_name text,
    "ID" text NOT NULL
);


ALTER TABLE public.bills OWNER TO postgres;

--
-- Name: monthlyBilling; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."monthlyBilling" (
    name text NOT NULL,
    price integer,
    user_name text,
    "ID" text NOT NULL
);


ALTER TABLE public."monthlyBilling" OWNER TO postgres;

--
-- Name: salaryStatements; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."salaryStatements" (
    salary integer,
    savings integer,
    name text
);


ALTER TABLE public."salaryStatements" OWNER TO postgres;

--
-- Name: userProfile; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."userProfile" (
    name text,
    age integer,
    email text,
    image text,
    password text
);


ALTER TABLE public."userProfile" OWNER TO postgres;

--
-- Data for Name: bills; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.bills (date, name, price, user_name, "ID") FROM stdin;
\.


--
-- Data for Name: monthlyBilling; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."monthlyBilling" (name, price, user_name, "ID") FROM stdin;
\.


--
-- Data for Name: salaryStatements; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."salaryStatements" (salary, savings, name) FROM stdin;
\.


--
-- Data for Name: userProfile; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."userProfile" (name, age, email, image, password) FROM stdin;
\.


--
-- Name: bills bills_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.bills
    ADD CONSTRAINT bills_pkey PRIMARY KEY ("ID");


--
-- Name: monthlyBilling monthlyBilling_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."monthlyBilling"
    ADD CONSTRAINT "monthlyBilling_pkey" PRIMARY KEY ("ID");


--
-- Name: userProfile unique_user_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."userProfile"
    ADD CONSTRAINT unique_user_name UNIQUE (name);


--
-- Name: monthlyBilling update_id_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_id_trigger BEFORE INSERT OR UPDATE ON public."monthlyBilling" FOR EACH ROW EXECUTE FUNCTION public.update_id();


--
-- PostgreSQL database dump complete
--

