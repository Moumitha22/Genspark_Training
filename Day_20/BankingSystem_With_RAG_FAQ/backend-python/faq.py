from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from llama_index.core import VectorStoreIndex, Document, Settings, load_index_from_storage
from llama_index.core import StorageContext
from llama_index.embeddings.huggingface import HuggingFaceEmbedding
from llama_index.llms.groq import Groq
import os
import json
from dotenv import load_dotenv

load_dotenv()

PERSIST_DIR = "faq_index" # folder in disc to store index(embeded vector)
FAQ_JSON_FILE = "faq.json"

# Setup LLM and embedding model once
Settings.llm = Groq(model="meta-llama/llama-4-scout-17b-16e-instruct", api_key=os.getenv("GROQ_API_KEY"))
Settings.embed_model = HuggingFaceEmbedding(model_name="all-MiniLM-L6-v2")

app = FastAPI()

class QuestionRequest(BaseModel):
    question: str

def load_faqs_from_json(json_file=FAQ_JSON_FILE):
    if not os.path.exists(json_file):
        raise FileNotFoundError(f"FAQ JSON file '{json_file}' not found.")
    with open(json_file, "r", encoding="utf-8") as f:
        faq_data = json.load(f)
    # Convert each Q&A pair into a single Document string (Question + Answer)
    documents = []
    for entry in faq_data:
        q = entry.get("question", "").strip()
        a = entry.get("answer", "").strip()
        if q and a:
            doc_text = f"Question: {q}\nAnswer: {a}"
            documents.append(Document(text=doc_text))
    return documents

def build_or_load_index():
    if os.path.exists(PERSIST_DIR):
        # Load existing index
        storage_context = StorageContext.from_defaults(persist_dir=PERSIST_DIR)
        index = load_index_from_storage(storage_context)
    else:
        # Build new index from JSON FAQs
        documents = load_faqs_from_json()
        index = VectorStoreIndex.from_documents(documents)
        index.storage_context.persist(persist_dir=PERSIST_DIR)
    return index

# Build/load index once at startup
index = build_or_load_index()

@app.post("/ask")
async def ask_faq(request: QuestionRequest):
    if not request.question.strip():
        raise HTTPException(status_code=400, detail="Question cannot be empty.")
    query_engine = index.as_query_engine()
    response = query_engine.query(request.question)
    return {"answer": response.response}

