// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Sqlite3ProducerDbBase is an abstract base class the storage of keys for the
	/// producer. It contains one table that maps time slots (to the nearest hour) to
	/// the content key created for that time slot. A subclass must implement the
	/// methods. For example, see Sqlite3ProducerDb. This base class has protected
	/// SQL strings and helpers so the subclasses can work with similar tables using
	/// their own SQLite libraries.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public abstract class Sqlite3ProducerDbBase : ProducerDb {
		protected internal const String INITIALIZATION1 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  contentkeys(                                     \n"
				+ "    rowId            INTEGER PRIMARY KEY,          \n"
				+ "    timeslot         INTEGER,                      \n"
				+ "    key              BLOB NOT NULL                 \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION2 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "   timeslotIndex ON contentkeys(timeslot);         \n";
	
		protected internal const String SELECT_hasContentKey = "SELECT key FROM contentkeys where timeslot=?";
		protected internal const String SELECT_getContentKey = "SELECT key FROM contentkeys where timeslot=?";
		protected internal const String INSERT_addContentKey = "INSERT INTO contentkeys (timeslot, key) values (?, ?)";
		protected internal const String DELETE_deleteContentKey = "DELETE FROM contentkeys WHERE timeslot=?";
	}
}
