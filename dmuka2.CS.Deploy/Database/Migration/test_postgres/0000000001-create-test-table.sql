--
-- PostgreSQL database dump
--

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 197 (class 1259 OID 17069)
-- Name: test_table; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.test_table (
    test_id bigint NOT NULL,
    test_col character varying(50) NOT NULL
);


ALTER TABLE public.test_table OWNER TO postgres;

--
-- TOC entry 2802 (class 0 OID 0)
-- Dependencies: 197
-- Name: TABLE test_table; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.test_table IS 'This table is only for tests.';


--
-- TOC entry 196 (class 1259 OID 17067)
-- Name: test_table_test_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.test_table_test_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.test_table_test_id_seq OWNER TO postgres;

--
-- TOC entry 2803 (class 0 OID 0)
-- Dependencies: 196
-- Name: test_table_test_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.test_table_test_id_seq OWNED BY public.test_table.test_id;


--
-- TOC entry 2670 (class 2604 OID 17072)
-- Name: test_table test_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.test_table ALTER COLUMN test_id SET DEFAULT nextval('public.test_table_test_id_seq'::regclass);


--
-- TOC entry 2672 (class 2606 OID 17074)
-- Name: test_table test_table_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.test_table
    ADD CONSTRAINT test_table_pkey PRIMARY KEY (test_id);


-- Completed on 2019-03-30 18:29:18

--
-- PostgreSQL database dump complete
--

