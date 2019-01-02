// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Sqlite3GroupManagerDbBase is an abstract base class for the storage of data
	/// used by the GroupManager. It contains two tables to store Schedules and
	/// Members. A subclass must implement the methods. For example, see
	/// Sqlite3GroupManagerDb. This base class has protected SQL strings and helpers
	/// so the subclasses can work with similar tables using their own SQLite
	/// libraries.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public abstract class Sqlite3GroupManagerDbBase : GroupManagerDb {
		/// <summary>
		/// Throw an exception if it is an error for addSchedule to add the schedule.
		/// </summary>
		///
		/// <param name="name">The name of the schedule. The name cannot be empty.</param>
		/// <exception cref="GroupManagerDb.Error">if the name is empty.</exception>
		protected static internal void checkAddSchedule(String name) {
			if (name.Length == 0)
				throw new GroupManagerDb.Error(
						"addSchedule: The schedule name cannot be empty");
		}
	
		/// <summary>
		/// Throw an exception if it is an error for renameSchedule to rename the
		/// schedule.
		/// </summary>
		///
		/// <param name="newName">The new name of the schedule. The name cannot be empty.</param>
		/// <exception cref="GroupManagerDb.Error">if newName is empty.</exception>
		protected static internal void checkRenameSchedule(String newName) {
			if (newName.Length == 0)
				throw new GroupManagerDb.Error(
						"renameSchedule: The schedule newName cannot be empty");
		}
	
		protected internal const String INITIALIZATION1 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  schedules(                                       \n"
				+ "    schedule_id         INTEGER PRIMARY KEY,       \n"
				+ "    schedule_name       TEXT NOT NULL,             \n"
				+ "    schedule            BLOB NOT NULL              \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION2 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "   scheduleNameIndex ON schedules(schedule_name);  \n";
	
		protected internal const String INITIALIZATION3 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  members(                                         \n"
				+ "    member_id           INTEGER PRIMARY KEY,       \n"
				+ "    schedule_id         INTEGER NOT NULL,          \n"
				+ "    member_name         BLOB NOT NULL,             \n"
				+ "    key_name            BLOB NOT NULL,             \n"
				+ "    pubkey              BLOB NOT NULL              \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION4 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "   memNameIndex ON members(member_name);           \n";
	
		protected internal const String INITIALIZATION5 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  ekeys(                                           \n"
				+ "    ekey_id             INTEGER PRIMARY KEY,       \n"
				+ "    ekey_name           BLOB NOT NULL,             \n"
				+ "    pub_key             BLOB NOT NULL              \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION6 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "   ekeyNameIndex ON ekeys(ekey_name);              \n";
	
		protected internal const String SELECT_hasSchedule = "SELECT schedule_id FROM schedules where schedule_name=?";
		protected internal const String SELECT_listAllScheduleNames = "SELECT schedule_name FROM schedules";
		protected internal const String SELECT_getSchedule = "SELECT schedule FROM schedules WHERE schedule_name=?";
		protected internal const String SELECT_getScheduleMembers = "SELECT key_name, pubkey "
				+ "FROM members JOIN schedules ON members.schedule_id=schedules.schedule_id "
				+ "WHERE schedule_name=?";
		protected internal const String INSERT_addSchedule = "INSERT INTO schedules (schedule_name, schedule) values (?, ?)";
		protected internal const String DELETE_deleteScheduleMembers = "DELETE FROM members WHERE schedule_id=?";
		protected internal const String DELETE_deleteSchedule = "DELETE FROM schedules WHERE schedule_id=?";
		protected internal const String WHERE_renameSchedule = "schedule_name=?";
		protected internal const String UPDATE_renameSchedule = "UPDATE schedules SET schedule_name=? WHERE "
				+ WHERE_renameSchedule;
		protected internal const String WHERE_updateSchedule = "schedule_name=?";
		protected internal const String UPDATE_updateSchedule = "UPDATE schedules SET schedule=? WHERE "
				+ WHERE_updateSchedule;
		protected internal const String SELECT_getScheduleId = "SELECT schedule_id FROM schedules WHERE schedule_name=?";
	
		protected internal const String SELECT_hasMember = "SELECT member_id FROM members WHERE member_name=?";
		protected internal const String SELECT_listAllMembers = "SELECT member_name FROM members";
		protected internal const String SELECT_getMemberSchedule = "SELECT schedule_name "
				+ "FROM schedules JOIN members ON schedules.schedule_id = members.schedule_id "
				+ "WHERE member_name=?";
		protected internal const String INSERT_addMember = "INSERT INTO members(schedule_id, member_name, key_name, pubkey) "
				+ "values (?, ?, ?, ?)";
		protected internal const String UPDATE_updateMemberSchedule = "UPDATE members SET schedule_id=? WHERE member_name=?";
		protected internal const String DELETE_deleteMember = "DELETE FROM members WHERE member_name=?";
	
		protected internal const String SELECT_hasEKey = "SELECT ekey_id FROM ekeys where ekey_name=?";
		protected internal const String INSERT_addEKey = "INSERT INTO ekeys(ekey_name, pub_key) values (?, ?)";
		protected internal const String SELECT_getEKey = "SELECT * FROM ekeys where ekey_name=?";
		protected internal const String DELETE_cleanEKeys = "DELETE FROM ekeys";
		protected internal const String DELETE_deleteEKey = "DELETE FROM ekeys WHERE ekey_name=?";
	}
}
